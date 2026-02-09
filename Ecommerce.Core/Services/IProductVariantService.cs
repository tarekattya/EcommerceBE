namespace Ecommerce.Core;

public interface IProductVariantService
{
    Task<IReadOnlyList<ProductVariantResponse>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);
    Task<Result<ProductVariantResponse>> CreateAsync(int productId, ProductVariantRequest request, CancellationToken cancellationToken = default);
    Task<Result<ProductVariantResponse>> UpdateAsync(int variantId, ProductVariantRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int variantId, CancellationToken cancellationToken = default);
}
