using Microsoft.EntityFrameworkCore.Query;

namespace Ecommerce.Tests.Concurrency;

public class DashboardConcurrencyTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Func<ApplicationDbContext> _dbContextFactory;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IOptions<BaseUrl>> _baseUrlOptionsMock;

    public DashboardConcurrencyTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        
        // Create factory for in-memory database instances (thread-safe)
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContextFactory = () => new ApplicationDbContext(options, Mock.Of<IHttpContextAccessor>());
        
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null!, null!, null!, null!, null!, null!, null!, null!
        );
        
        _baseUrlOptionsMock = new Mock<IOptions<BaseUrl>>();
        _baseUrlOptionsMock.Setup(x => x.Value).Returns(new BaseUrl { BaseURL = "https://localhost:7021" });
    }
    
    private DashboardService CreateDashboardService(ApplicationDbContext dbContext)
    {
        // Create a new UserManager mock with its own Users DbSet for thread safety
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null!, null!, null!, null!, null!, null!, null!, null!
        );
        var usersDbContext = _dbContextFactory();
        userManagerMock.Setup(x => x.Users).Returns(usersDbContext.Users);
        
        return new DashboardService(
            _unitOfWorkMock.Object,
            dbContext,
            userManagerMock.Object,
            _baseUrlOptionsMock.Object
        );
    }

    [Fact]
    public async Task GetDashboardStatsAsync_ShouldHandleConcurrentRequests_WithoutDeadlock()
    {
        // Arrange
        var orderRepoMock = new Mock<IGenericRepository<Order>>();
        var productRepoMock = new Mock<IGenericRepository<Product>>();

        _unitOfWorkMock.Setup(x => x.Repository<Order>()).Returns(orderRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.Repository<Product>()).Returns(productRepoMock.Object);

        orderRepoMock.Setup(x => x.GetCountAsync(It.IsAny<BaseSpecifications<Order>>()))
            .ReturnsAsync(10);
        orderRepoMock.Setup(x => x.GetAllWithSpecAsync(It.IsAny<RecentOrdersSpec>()))
            .ReturnsAsync(new List<Order>());

        productRepoMock.Setup(x => x.GetCountAsync(It.IsAny<BaseSpecifications<Product>>()))
            .ReturnsAsync(50);
        productRepoMock.Setup(x => x.GetAllWithSpecAsync(It.IsAny<LowStockProductSpec>()))
            .ReturnsAsync(new List<Product>());

        // Act - Launch 100 concurrent requests, each with its own DbContext instance
        const int concurrentRequests = 100;
        var tasks = Enumerable.Range(0, concurrentRequests)
            .Select(async i =>
            {
                var dbContext = _dbContextFactory();
                var service = CreateDashboardService(dbContext);
                try
                {
                    return await service.GetDashboardStatsAsync();
                }
                finally
                {
                    await dbContext.DisposeAsync();
                }
            })
            .ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert - All requests should complete successfully
        results.Should().AllSatisfy(result =>
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
        });
    }

    [Fact]
    public async Task GetDashboardStatsAsync_ShouldHandleRaceConditions_WithThreadSafety()
    {
        // Arrange
        var orderRepoMock = new Mock<IGenericRepository<Order>>();
        var productRepoMock = new Mock<IGenericRepository<Product>>();

        _unitOfWorkMock.Setup(x => x.Repository<Order>()).Returns(orderRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.Repository<Product>()).Returns(productRepoMock.Object);

        // Simulate varying response times
        var callCount = 0;
        orderRepoMock.Setup(x => x.GetCountAsync(It.IsAny<BaseSpecifications<Order>>()))
            .ReturnsAsync(() => Interlocked.Increment(ref callCount));

        orderRepoMock.Setup(x => x.GetAllWithSpecAsync(It.IsAny<OrderWithItemsSpec>()))
            .ReturnsAsync(new List<Order>());
        orderRepoMock.Setup(x => x.GetAllWithSpecAsync(It.IsAny<RecentOrdersSpec>()))
            .ReturnsAsync(new List<Order>());

        productRepoMock.Setup(x => x.GetCountAsync(It.IsAny<BaseSpecifications<Product>>()))
            .ReturnsAsync(50);
        productRepoMock.Setup(x => x.GetAllWithSpecAsync(It.IsAny<LowStockProductSpec>()))
            .ReturnsAsync(new List<Product>());

        // Act - Execute concurrent requests with delays, each with its own DbContext
        const int concurrentRequests = 50;
        var tasks = Enumerable.Range(0, concurrentRequests)
            .Select(async i =>
            {
                await Task.Delay(Random.Shared.Next(0, 10));
                var dbContext = _dbContextFactory();
                var service = CreateDashboardService(dbContext);
                try
                {
                    return await service.GetDashboardStatsAsync();
                }
                finally
                {
                    await dbContext.DisposeAsync();
                }
            })
            .ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().HaveCount(concurrentRequests);
        results.Should().AllSatisfy(result => result.IsSuccess.Should().BeTrue());
    }

    [Fact]
    public async Task GetDashboardStatsAsync_ShouldCompleteQuickly_UnderLoad()
    {
        // Arrange
        var orderRepoMock = new Mock<IGenericRepository<Order>>();
        var productRepoMock = new Mock<IGenericRepository<Product>>();

        _unitOfWorkMock.Setup(x => x.Repository<Order>()).Returns(orderRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.Repository<Product>()).Returns(productRepoMock.Object);

        orderRepoMock.Setup(x => x.GetCountAsync(It.IsAny<BaseSpecifications<Order>>()))
            .ReturnsAsync(10);
        orderRepoMock.Setup(x => x.GetAllWithSpecAsync(It.IsAny<RecentOrdersSpec>()))
            .ReturnsAsync(new List<Order>());

        productRepoMock.Setup(x => x.GetCountAsync(It.IsAny<BaseSpecifications<Product>>()))
            .ReturnsAsync(50);
        productRepoMock.Setup(x => x.GetAllWithSpecAsync(It.IsAny<LowStockProductSpec>()))
            .ReturnsAsync(new List<Product>());

        // Act - Measure performance under load, each with its own DbContext
        const int loadLevel = 20;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var tasks = Enumerable.Range(0, loadLevel)
            .Select(async i =>
            {
                var dbContext = _dbContextFactory();
                var service = CreateDashboardService(dbContext);
                try
                {
                    return await service.GetDashboardStatsAsync();
                }
                finally
                {
                    await dbContext.DisposeAsync();
                }
            })
            .ToArray();

        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert - Should complete all requests reasonably quickly
        results.Should().HaveCount(loadLevel);
        results.Should().AllSatisfy(r => r.IsSuccess.Should().BeTrue());
        
        // Average time per request should be reasonable (with mocked repositories)
        var averageTimePerRequest = stopwatch.ElapsedMilliseconds / (double)loadLevel;
        averageTimePerRequest.Should().BeLessThan(100); // ms per request
    }

}
