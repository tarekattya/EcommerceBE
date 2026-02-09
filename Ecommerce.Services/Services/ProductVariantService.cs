namespace Ecommerce.Application;

public class ProductVariantService(IUnitOfWork unitOfWork) : IProductVariantService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<ProductVariantResponse>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        var variants = await _unitOfWork.Repository<ProductVariant>()
            .GetAllWithSpecAsync(new ProductVariantsByProductIdSpec(productId));
        return variants.Adapt<IReadOnlyList<ProductVariantResponse>>();
    }

    public async Task<Result<ProductVariantResponse>> CreateAsync(int productId, ProductVariantRequest request, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
        if (product is null)
            return Result<ProductVariantResponse>.Failure(ProductErrors.NotFoundProduct);

        var variant = new ProductVariant(productId, request.Size, request.Color, request.SKU, request.Price, request.Stock);
        await _unitOfWork.Repository<ProductVariant>().AddAsync(variant);
        await _unitOfWork.CompleteAsync();
        return Result<ProductVariantResponse>.Success(variant.Adapt<ProductVariantResponse>());
    }

    public async Task<Result<ProductVariantResponse>> UpdateAsync(int variantId, ProductVariantRequest request, CancellationToken cancellationToken = default)
    {
        var variant = await _unitOfWork.Repository<ProductVariant>().GetByIdAsync(variantId);
        if (variant is null)
            return Result<ProductVariantResponse>.Failure(ProductErrors.NotFoundProduct);

        variant.Size = request.Size;
        variant.Color = request.Color;
        variant.SKU = request.SKU;
        variant.Price = request.Price;
        variant.UpdateStock(request.Stock);
        _unitOfWork.Repository<ProductVariant>().Update(variant);
        await _unitOfWork.CompleteAsync();
        return Result<ProductVariantResponse>.Success(variant.Adapt<ProductVariantResponse>());
    }

    public async Task<Result> DeleteAsync(int variantId, CancellationToken cancellationToken = default)
    {
        var variant = await _unitOfWork.Repository<ProductVariant>().GetByIdAsync(variantId);
        if (variant is null)
            return Result.Failure(ProductErrors.NotFoundProduct);
        _unitOfWork.Repository<ProductVariant>().Delete(variant);
        await _unitOfWork.CompleteAsync();
        return Result.Success();
    }
}
