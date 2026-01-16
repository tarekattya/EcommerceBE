using Ecommerce.Shared;

namespace Ecommerce.Application;

public class CouponService(IUnitOfWork unitOfWork) : ICouponService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<CouponResponse>> GetCouponByCodeAsync(string code)
    {
        var coupon = await _unitOfWork.Repository<Coupon>().GetByIdWithSpecAsync(new CouponByCodeSpec(code));
        
        if (coupon == null)
            return Result<CouponResponse>.Failure(new Error("Coupon.NotFound", "Coupon not found", StatusCodes.Status404NotFound));

        return Result<CouponResponse>.Success(coupon.Adapt<CouponResponse>());
    }

    public async Task<Result<CouponResponse>> CreateCouponAsync(CouponRequest request)
    {
        var exists = await _unitOfWork.Repository<Coupon>().GetCountAsync(new CouponByCodeSpec(request.Code));
        if (exists > 0)
            return Result<CouponResponse>.Failure(new Error("Coupon.Duplicate", "Coupon with this code already exists", StatusCodes.Status400BadRequest));

        var coupon = new Coupon(
            request.Code,
            request.DiscountValue,
            (DiscountType)request.DiscountType,
            request.ExpiryDate,
            request.MinimumAmount,
            request.UsageLimit
        );

        await _unitOfWork.Repository<Coupon>().AddAsync(coupon);
        await _unitOfWork.CompleteAsync();

        return Result<CouponResponse>.Success(coupon.Adapt<CouponResponse>());
    }

    public async Task<Result> DeleteCouponAsync(int id)
    {
        var coupon = await _unitOfWork.Repository<Coupon>().GetByIdAsync(id);
        if (coupon == null)
            return Result.Failure(new Error("Coupon.NotFound", "Coupon not found", StatusCodes.Status404NotFound));

        _unitOfWork.Repository<Coupon>().Delete(coupon);
        await _unitOfWork.CompleteAsync();

        return Result.Success();
    }

    public async Task<Result<IReadOnlyList<CouponResponse>>> GetAllCouponsAsync()
    {
        var coupons = await _unitOfWork.Repository<Coupon>().GetAllAsync();
        return Result<IReadOnlyList<CouponResponse>>.Success(coupons.Adapt<IReadOnlyList<CouponResponse>>());
    }

    public async Task<Result<decimal>> ValidateAndApplyCouponAsync(string code, decimal currentOrderAmount, decimal shippingCost = 0, IEnumerable<OrderItem>? items = null)
    {
        var coupon = await _unitOfWork.Repository<Coupon>().GetByIdWithSpecAsync(new CouponByCodeSpec(code));

        if (coupon == null)
            return Result<decimal>.Failure(new Error("Coupon.NotFound", "Invalid coupon code", StatusCodes.Status400BadRequest));

        if (!coupon.IsValid())
            return Result<decimal>.Failure(new Error("Coupon.Invalid", "Coupon is expired or inactive", StatusCodes.Status400BadRequest));

        if (coupon.MinimumAmount.HasValue && currentOrderAmount < coupon.MinimumAmount.Value)
            return Result<decimal>.Failure(new Error("Coupon.MinAmount", $"Minimum order amount of {coupon.MinimumAmount.Value} required", StatusCodes.Status400BadRequest));

        var discount = coupon.CalculateDiscount(currentOrderAmount, shippingCost, items);
        
        // Note: UsageCount increment should happen when order is actually placed, 
        // but for real-time validation we just return the discount amount.
        
        return Result<decimal>.Success(discount);
    }
}
