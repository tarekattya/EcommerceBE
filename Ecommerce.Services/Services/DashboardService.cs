namespace Ecommerce.Application;

public class DashboardService(IUnitOfWork unitOfWork, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, IOptions<BaseUrl> baseUrlOptions) : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly string _baseUrl = baseUrlOptions.Value.BaseURL;

    public async Task<Result<DashboardDto>> GetDashboardStatsAsync()
    {
        IGenericRepository<Order>? orderRepo = _unitOfWork.Repository<Order>();
        IGenericRepository<Product>? productRepo = _unitOfWork.Repository<Product>();

        Task<int>? pendingTask = orderRepo.GetCountAsync(new BaseSpecifications<Order>(o => o.Status == OrderStatus.Pending));
        Task<int>? failedTask = orderRepo.GetCountAsync(new BaseSpecifications<Order>(o => o.Status == OrderStatus.PaymentFailed));
        Task<int>? processingTask = orderRepo.GetCountAsync(new BaseSpecifications<Order>(o => o.Status == OrderStatus.Processing));
        Task<int>? completedTask = orderRepo.GetCountAsync(new BaseSpecifications<Order>(o => o.Status == OrderStatus.Delivered));
        Task<int>? cancelledTask = orderRepo.GetCountAsync(new BaseSpecifications<Order>(o => o.Status == OrderStatus.Cancelled));
        Task<int>? totalOrdersTask = orderRepo.GetCountAsync(new BaseSpecifications<Order>());
        
        Task<decimal?>? revenueTask = _dbContext.Orders
            .Where(o => !o.IsDeleted && 
                       o.Status != OrderStatus.Cancelled && 
                       o.Status != OrderStatus.PaymentFailed)
            .SumAsync(o => (decimal?)o.SubTotal);
        
        Task<List<TopProductDto>>? topSellingTask = GetTopSellingProductsAsync();
        
        Task<IReadOnlyList<Order>>? recentOrdersTask = orderRepo.GetAllWithSpecAsync(new RecentOrdersSpec(5));
        Task<int>? totalProductsTask = productRepo.GetCountAsync(new BaseSpecifications<Product>());
        Task<IReadOnlyList<Product>>? lowStockProductsTask = productRepo.GetAllWithSpecAsync(new LowStockProductSpec(10));
        Task<int>? totalUsersTask = _userManager.Users.CountAsync();

        await Task.WhenAll(
            pendingTask,
            failedTask,
            processingTask,
            completedTask,
            cancelledTask,
            totalOrdersTask,
            revenueTask,
            topSellingTask,
            recentOrdersTask,
            totalProductsTask,
            lowStockProductsTask,
            totalUsersTask
        );

        int pending = await pendingTask;
        int failed = await failedTask;
        int processing = await processingTask;
        int completed = await completedTask;
        int cancelled = await cancelledTask;
        int total = await totalOrdersTask;
        decimal revenue = await revenueTask ?? 0m;
        List<TopProductDto> topSelling = await topSellingTask;
        IReadOnlyList<Order>? recentOrdersList = await recentOrdersTask;
        int totalProducts = await totalProductsTask;
        IReadOnlyList<Product>? lowStockProductsList = await lowStockProductsTask;
        int totalUsers = await totalUsersTask;

        List<LowStockProductDto> lowStockDtos = new();
        if (lowStockProductsList is not null)
        {
            lowStockDtos = lowStockProductsList.Select(p => new LowStockProductDto(
                p.Id,
                p.Name,
                MapPictureUrl(p.PictureUrl),
                p.Stock,
                p.Category?.Name ?? "N/A"
            )).ToList();
        }

        List<RecentOrderDto> recentOrderDtos = new List<RecentOrderDto>();
        if (recentOrdersList is not null)
        {
            recentOrderDtos = recentOrdersList
                .Select(o => new RecentOrderDto(
                    o.Id,
                    o.OrderDate, 
                    o.BuyerEmail,
                    o.GetTotal(),
                    o.Status.ToString()
                ))
                .ToList();
        }

        return Result<DashboardDto>.Success(new DashboardDto(
            total, 
            revenue, 
            pending, 
            failed, 
            processing,
            completed, 
            cancelled, 
            totalProducts, 
            totalUsers,
            topSelling,
            lowStockDtos,
            recentOrderDtos
        ));
    }

    private async Task<List<TopProductDto>> GetTopSellingProductsAsync()
    {
        var topProducts = await _dbContext.Orders
            .Where(o => !o.IsDeleted && 
                       o.Status != OrderStatus.Cancelled && 
                       o.Status != OrderStatus.PaymentFailed)
            .SelectMany(o => o.Items)
            .Where(oi => oi.ProductItemOrderd != null)
            .GroupBy(oi => new
            {
                oi.ProductItemOrderd!.ProductId,
                oi.ProductItemOrderd.Name,
                oi.ProductItemOrderd.PictureUrl
            })
            .Select(g => new
            {
                ProductId = g.Key.ProductId,
                Name = g.Key.Name,
                PictureUrl = g.Key.PictureUrl,
                QuantitySold = g.Sum(oi => oi.Quantity),
                Revenue = g.Sum(oi => oi.Price * oi.Quantity)
            })
            .OrderByDescending(x => x.QuantitySold)
            .Take(5)
            .ToListAsync();

        return topProducts.Select(p => new TopProductDto(
            p.ProductId,
            p.Name ?? "Unknown",
            MapPictureUrl(p.PictureUrl),
            p.QuantitySold,
            p.Revenue
        )).ToList();
    }

    private string MapPictureUrl(string? pictureUrl)
    {
        if (string.IsNullOrEmpty(pictureUrl)) return string.Empty;
        if (pictureUrl.StartsWith("http")) return pictureUrl;
        return $"{_baseUrl}/{pictureUrl.TrimStart('/')}";
    }
}

