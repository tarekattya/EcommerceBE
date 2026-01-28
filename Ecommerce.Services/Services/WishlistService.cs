namespace Ecommerce.Application;

public class WishlistService(IUnitOfWork unitOfWork) : IWishlistService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<IReadOnlyList<WishlistItemResponse>>> GetUserWishlistAsync(string userId)
    {
        WishlistByUserSpec? spec = new WishlistByUserSpec(userId);
        IReadOnlyList<WishlistItem>? items = await _unitOfWork.Repository<WishlistItem>().GetAllWithSpecAsync(spec);

        List<WishlistItemResponse>? response = items.Select(item => new WishlistItemResponse(
            item.Id,
            item.ProductId,
            item.Product.Name,
            item.Product.PictureUrl,
            item.Product.Price,
            item.Product.Brand.Name,
            item.Product.Category.Name,
            item.CreatedAt
        )).ToList();

        return Result<IReadOnlyList<WishlistItemResponse>>.Success(response);
    }
    public async Task<Result<WishlistItemResponse>> AddToWishlistAsync(string userId, WishlistItemRequest request)
    {
        Product? product = await _unitOfWork.Repository<Product>()
            .GetByIdWithSpecAsync(new ProductSpecWithBrandAndCategory(request.ProductId));
        
        if (product is null)
            return Result<WishlistItemResponse>.Failure(WishlistErrors.ProductNotFound);

        WishlistItemByUserAndProductSpec? existingSpec = new WishlistItemByUserAndProductSpec(userId, request.ProductId);
        WishlistItem? existingItem = await _unitOfWork.Repository<WishlistItem>().GetByIdWithSpecAsync(existingSpec);
        
        if (existingItem is not null)
            return Result<WishlistItemResponse>.Failure(WishlistErrors.ItemAlreadyExists);

        var wishlistItem = new WishlistItem(userId, request.ProductId);
        WishlistItem? created = await _unitOfWork.Repository<WishlistItem>().AddAsync(wishlistItem);
        await _unitOfWork.CompleteAsync();

        WishlistItemResponse? response = new WishlistItemResponse(
            created.Id,
            product.Id,
            product.Name,
            product.PictureUrl,
            product.Price,
            product.Brand.Name,
            product.Category.Name,
            created.CreatedAt
        );

        return Result<WishlistItemResponse>.Success(response);
    }

    public async Task<Result> RemoveFromWishlistAsync(string userId, int productId)
    {
        WishlistItemByUserAndProductSpec? spec = new WishlistItemByUserAndProductSpec(userId, productId);
        WishlistItem? item = await _unitOfWork.Repository<WishlistItem>().GetByIdWithSpecAsync(spec);

        if (item is null)
            return Result.Failure(WishlistErrors.ItemNotFound);

        _unitOfWork.Repository<WishlistItem>().Delete(item);
        await _unitOfWork.CompleteAsync();

        return Result.Success();
    }

    public async Task<Result<bool>> IsInWishlistAsync(string userId, int productId)
    {
        WishlistItemByUserAndProductSpec? spec = new WishlistItemByUserAndProductSpec(userId, productId);
        WishlistItem? item = await _unitOfWork.Repository<WishlistItem>().GetByIdWithSpecAsync(spec);

        return Result<bool>.Success(item is not null);
    }

    public async Task<Result> ClearWishlistAsync(string userId)
    {
        WishlistByUserSpec? spec = new WishlistByUserSpec(userId);
        IReadOnlyList<WishlistItem>? items = await _unitOfWork.Repository<WishlistItem>().GetAllWithSpecAsync(spec);

        foreach (WishlistItem item in items)
        {
            _unitOfWork.Repository<WishlistItem>().Delete(item);
        }

        await _unitOfWork.CompleteAsync();
        return Result.Success();
    }
}
