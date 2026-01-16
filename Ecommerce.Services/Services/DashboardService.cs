namespace Ecommerce.Services;

public class DashboardService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager) : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<DashboardDto>> GetDashboardStatsAsync()
    {
        var repo = _unitOfWork.Repository<Order>();

        var pendingSpec = new BaseSpecifications<Order>(o => o.Status == OrderStatus.Pending);
        int pending = await repo.GetCountAsync(pendingSpec);

        var failedSpec = new BaseSpecifications<Order>(o => o.Status == OrderStatus.PaymentFailed);
        int failed = await repo.GetCountAsync(failedSpec);

        var completedSpec = new BaseSpecifications<Order>(o => o.Status == OrderStatus.Delivered);
        int completed = await repo.GetCountAsync(completedSpec);

        var cancelledSpec = new BaseSpecifications<Order>(o => o.Status == OrderStatus.Cancelled);
        int cancelled = await repo.GetCountAsync(cancelledSpec);

        var allSpec = new BaseSpecifications<Order>();
        int total = await repo.GetCountAsync(allSpec);

        var revenueSpec = new BaseSpecifications<Order>(o => o.Status != OrderStatus.Cancelled && o.Status != OrderStatus.PaymentFailed);
        var validOrders = await repo.GetAllWithSpecAsync(revenueSpec);
        decimal revenue = validOrders.Sum(o => o.SubTotal);

        var productRepo = _unitOfWork.Repository<Product>();
        var productSpec = new BaseSpecifications<Product>();
        int totalProducts = await productRepo.GetCountAsync(productSpec);

        int totalUsers = await _userManager.Users.CountAsync();

        return Result<DashboardDto>.Success(new DashboardDto(total, revenue, pending, failed, completed, cancelled, totalProducts, totalUsers));
    }
}
