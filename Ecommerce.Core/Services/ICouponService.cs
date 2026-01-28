using Ecommerce.Shared;

namespace Ecommerce.Core;

public interface ICouponService
{
    Task<Result<CouponResponse>> GetCouponByCodeAsync(string code);
    Task<Result<CouponResponse>> CreateCouponAsync(CouponRequest request);
    Task<Result> DeleteCouponAsync(int id);
    Task<Result<Pagination<CouponResponse>>> GetAllCouponsAsync(CouponSpecParams specParams);
    Task<Result<decimal>> ValidateAndApplyCouponAsync(string code, decimal currentOrderAmount, decimal shippingCost = 0, IEnumerable<OrderItem>? items = null);
}
