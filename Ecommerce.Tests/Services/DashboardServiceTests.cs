using Microsoft.EntityFrameworkCore.Query;

namespace Ecommerce.Tests.Services;

public class DashboardServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ApplicationDbContext _dbContext;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IOptions<BaseUrl>> _baseUrlOptionsMock;
    private readonly DashboardService _dashboardService;

    public DashboardServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        
        // Create in-memory database for testing
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new ApplicationDbContext(options, Mock.Of<IHttpContextAccessor>());
        
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null!, null!, null!, null!, null!, null!, null!, null!
        );
        
        _baseUrlOptionsMock = new Mock<IOptions<BaseUrl>>();
        _baseUrlOptionsMock.Setup(x => x.Value).Returns(new BaseUrl { BaseURL = "https://localhost:7021" });

        _dashboardService = new DashboardService(
            _unitOfWorkMock.Object,
            _dbContext,
            _userManagerMock.Object,
            _baseUrlOptionsMock.Object
        );
    }

    [Fact]
    public async Task GetDashboardStatsAsync_ShouldReturnSuccess_WhenAllDataExists()
    {
        // Arrange
        var orderRepoMock = new Mock<IGenericRepository<Order>>();
        var productRepoMock = new Mock<IGenericRepository<Product>>();

        _unitOfWorkMock.Setup(x => x.Repository<Order>()).Returns(orderRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.Repository<Product>()).Returns(productRepoMock.Object);

        // Setup DeliveryMethod first (required for Orders)
        var deliveryMethod = new DeliveryMethod("Standard", "Standard Delivery", 10, "3-5 days");
        _dbContext.DeliveryMethods.Add(deliveryMethod);
        
        // Set RowVersion for DeliveryMethod
        var deliveryMethodEntry = _dbContext.Entry(deliveryMethod);
        deliveryMethodEntry.Property(x => x.RowVersion).CurrentValue = new byte[8];
        deliveryMethodEntry.Property(x => x.RowVersion).IsModified = true;
        
        await _dbContext.SaveChangesAsync();
        
        // Setup DbContext Orders DbSet for revenue and top products queries
        var ordersData = new List<Order>
        {
            CreateTestOrder(1, OrderStatus.PaymentSucceeded, 100, CreateOrderItems(1), deliveryMethod),
            CreateTestOrder(2, OrderStatus.Delivered, 200, CreateOrderItems(2), deliveryMethod)
        };
        
        _dbContext.Orders.AddRange(ordersData);
        
        // Set RowVersion for all Orders and OrderItems
        foreach (var order in ordersData)
        {
            var orderEntry = _dbContext.Entry(order);
            orderEntry.Property(x => x.RowVersion).CurrentValue = new byte[8];
            orderEntry.Property(x => x.RowVersion).IsModified = true;
            
            foreach (var item in order.Items)
            {
                var itemEntry = _dbContext.Entry(item);
                itemEntry.Property(x => x.RowVersion).CurrentValue = new byte[8];
                itemEntry.Property(x => x.RowVersion).IsModified = true;
            }
        }
        
        await _dbContext.SaveChangesAsync();

        // Setup count queries
        orderRepoMock.Setup(x => x.GetCountAsync(It.IsAny<BaseSpecifications<Order>>()))
            .ReturnsAsync(10);


        // Setup recent orders
        var recentOrders = new List<Order>
        {
            CreateTestOrder(1, OrderStatus.Pending, 100, new List<OrderItem>(), deliveryMethod),
            CreateTestOrder(2, OrderStatus.Processing, 200, new List<OrderItem>(), deliveryMethod)
        };
        orderRepoMock.Setup(x => x.GetAllWithSpecAsync(It.IsAny<RecentOrdersSpec>()))
            .ReturnsAsync(recentOrders);

        // Setup products
        productRepoMock.Setup(x => x.GetCountAsync(It.IsAny<BaseSpecifications<Product>>()))
            .ReturnsAsync(50);
        productRepoMock.Setup(x => x.GetAllWithSpecAsync(It.IsAny<LowStockProductSpec>()))
            .ReturnsAsync(new List<Product>());

        // Setup users - use in-memory database Users DbSet which supports async operations
        _userManagerMock.Setup(x => x.Users).Returns(_dbContext.Users);

        // Act
        var result = await _dashboardService.GetDashboardStatsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.TotalOrders.Should().Be(10);
        result.Value.TotalRevenue.Should().Be(300); // 100 + 200
        result.Value.TopSellingProducts.Should().NotBeNull();
    }

    [Fact]
    public async Task GetDashboardStatsAsync_ShouldHandleNullOrders_Gracefully()
    {
        // Arrange
        var orderRepoMock = new Mock<IGenericRepository<Order>>();
        var productRepoMock = new Mock<IGenericRepository<Product>>();

        _unitOfWorkMock.Setup(x => x.Repository<Order>()).Returns(orderRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.Repository<Product>()).Returns(productRepoMock.Object);

        // Clear any existing orders in the in-memory database
        _dbContext.Orders.RemoveRange(_dbContext.Orders);
        await _dbContext.SaveChangesAsync();

        orderRepoMock.Setup(x => x.GetCountAsync(It.IsAny<BaseSpecifications<Order>>()))
            .ReturnsAsync(0);

        orderRepoMock.Setup(x => x.GetAllWithSpecAsync(It.IsAny<OrderWithItemsSpec>()))
            .ReturnsAsync((IReadOnlyList<Order>?)null);

        orderRepoMock.Setup(x => x.GetAllWithSpecAsync(It.IsAny<RecentOrdersSpec>()))
            .ReturnsAsync((IReadOnlyList<Order>?)null);

        productRepoMock.Setup(x => x.GetCountAsync(It.IsAny<BaseSpecifications<Product>>()))
            .ReturnsAsync(0);
        productRepoMock.Setup(x => x.GetAllWithSpecAsync(It.IsAny<LowStockProductSpec>()))
            .ReturnsAsync((IReadOnlyList<Product>?)null);

        // Setup users - use in-memory database Users DbSet which supports async operations
        _userManagerMock.Setup(x => x.Users).Returns(_dbContext.Users);

        // Act
        var result = await _dashboardService.GetDashboardStatsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalRevenue.Should().Be(0);
        result.Value.TopSellingProducts.Should().BeEmpty();
        result.Value.RecentOrders.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDashboardStatsAsync_ShouldExecuteQueriesInParallel()
    {
        // Arrange
        var orderRepoMock = new Mock<IGenericRepository<Order>>();
        var productRepoMock = new Mock<IGenericRepository<Product>>();

        _unitOfWorkMock.Setup(x => x.Repository<Order>()).Returns(orderRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.Repository<Product>()).Returns(productRepoMock.Object);

        // Clear any existing orders in the in-memory database
        _dbContext.Orders.RemoveRange(_dbContext.Orders);
        await _dbContext.SaveChangesAsync();

        var callOrder = new List<string>();
        var delay = TimeSpan.FromMilliseconds(100);

        // Setup delayed responses to verify parallel execution
        orderRepoMock.Setup(x => x.GetCountAsync(It.IsAny<BaseSpecifications<Order>>()))
            .Returns(async () =>
            {
                await Task.Delay(delay);
                callOrder.Add("count1");
                return 10;
            });

        orderRepoMock.Setup(x => x.GetAllWithSpecAsync(It.IsAny<OrderWithItemsSpec>()))
            .Returns(async () =>
            {
                await Task.Delay(delay);
                callOrder.Add("orders");
                return new List<Order>();
            });

        productRepoMock.Setup(x => x.GetCountAsync(It.IsAny<BaseSpecifications<Product>>()))
            .Returns(async () =>
            {
                await Task.Delay(delay);
                callOrder.Add("products");
                return 0;
            });

        orderRepoMock.Setup(x => x.GetAllWithSpecAsync(It.IsAny<RecentOrdersSpec>()))
            .ReturnsAsync(new List<Order>());
        productRepoMock.Setup(x => x.GetAllWithSpecAsync(It.IsAny<LowStockProductSpec>()))
            .ReturnsAsync(new List<Product>());
        // Setup users - use in-memory database Users DbSet which supports async operations
        _userManagerMock.Setup(x => x.Users).Returns(_dbContext.Users);

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await _dashboardService.GetDashboardStatsAsync();
        stopwatch.Stop();

        // Assert - If queries run in parallel, total time should be less than sequential time
        // Sequential would be ~300ms (3 queries * 100ms), parallel should be ~100ms
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(250);
        result.IsSuccess.Should().BeTrue();
    }

    private static Order CreateTestOrder(int id, OrderStatus status, decimal subTotal, ICollection<OrderItem> items, DeliveryMethod deliveryMethod)
    {
        var order = new Order(
            $"user{id}@test.com",
            new OrderAddress("City", "Street", "Country", "FirstName", "LastName"),
            deliveryMethod,
            items,
            subTotal
        );
        return order;
    }

    private static List<OrderItem> CreateOrderItems(int productId)
    {
        var productItem = new ProductItemOrderd
        {
            ProductId = productId,
            Name = $"Product{productId}",
            Description = "Description",
            PictureUrl = "image.jpg"
        };

        return new List<OrderItem>
        {
            new OrderItem(productItem, 50, 2)
        };
    }

}
