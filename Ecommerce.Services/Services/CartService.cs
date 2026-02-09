namespace Ecommerce.Application;

public class CartService(ICartRepository cartRepository, IUnitOfWork unitOfWork, IOptions<BaseUrl> baseUrlOptions) : ICartService
{
    private readonly ICartRepository _cartRepository = cartRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly string _baseUrl = baseUrlOptions.Value.BaseURL;

    public async Task<Result<CartResponse>> CreateCartAsync(CartRequest cartRequest)
    {
        if (string.IsNullOrWhiteSpace(cartRequest.Id))
            return Result<CartResponse>.Failure(CartErrors.InvalidCartId);

        CustomerCart? existingCart = await _cartRepository.GetCartAsync(cartRequest.Id);
        if (existingCart is not null)
            return Result<CartResponse>.Failure(CartErrors.InvalidCartId);

        if (cartRequest.Items is null || !cartRequest.Items.Any())
            return Result<CartResponse>.Failure(CartErrors.EmptyCart);

        CustomerCart? cart = new CustomerCart
        {
            Id = cartRequest.Id,
            Items = new List<CartItem>()
        };

        foreach (CartItemsRequest itemRequest in cartRequest.Items)
        {
            CartItem? cartItem = await ResolveCartItemAsync(itemRequest.VariantId, itemRequest.ProductId, itemRequest.Quantity);
            if (cartItem is null)
                return Result<CartResponse>.Failure(ProductErrors.NotFoundProduct);
            cart.Items.Add(cartItem);
        }

        cart.Items = cart.Items
            .GroupBy(i => i.VariantId)
            .Select(g =>
            {
                var first = g.First();
                first.Quantity = g.Sum(x => x.Quantity);
                return first;
            })
            .ToList();

        var createdCart = await _cartRepository.CreateCartAsync(cart);

        
        return createdCart is not null
            ? Result<CartResponse>.Success(createdCart.Adapt<CartResponse>())
            : Result<CartResponse>.Failure(CartErrors.CantCreateCart);
    }

    public async Task<Result<CartResponse>> UpdateCartAsync(CartRequest cartRequest)
    {
        if (string.IsNullOrWhiteSpace(cartRequest.Id))
            return Result<CartResponse>.Failure(CartErrors.InvalidCartId);

        var cart = await _cartRepository.GetCartAsync(cartRequest.Id);
        if (cart is null)
            return Result<CartResponse>.Failure(CartErrors.NotFoundCart);

        if (cartRequest.Items is null || !cartRequest.Items.Any())
            return Result<CartResponse>.Failure(CartErrors.EmptyCart);

        foreach (var item in cartRequest.Items)
        {
            var existingItem = cart.Items.FirstOrDefault(i => i.VariantId == item.VariantId);
            if (existingItem is null)
                continue;

            CartItem? resolved = await ResolveCartItemAsync(item.VariantId, item.ProductId, item.Quantity);
            if (resolved is null)
                return Result<CartResponse>.Failure(ProductErrors.NotFoundProduct);
            existingItem.Quantity = item.Quantity;
            existingItem.Price = resolved.Price;
            existingItem.ProductName = resolved.ProductName;
            existingItem.Brand = resolved.Brand;
            existingItem.Type = resolved.Type;
            existingItem.PictureUrl = resolved.PictureUrl;
            existingItem.Size = resolved.Size;
            existingItem.Color = resolved.Color;
        }

        var updatedCart = await _cartRepository.UpdateCartAsync(cart);

        return updatedCart is not null
            ? Result<CartResponse>.Success(updatedCart.Adapt<CartResponse>())
            : Result<CartResponse>.Failure(CartErrors.CantUpdateCart);
    }

    public async Task<Result<CartResponse>> GetCartAsync(string key)
    {
        var basket = await _cartRepository.GetCartAsync(key);
        if (basket is not null)
            return Result<CartResponse>.Success(basket.Adapt<CartResponse>());
        return Result<CartResponse>.Failure(CartErrors.NotFoundCart);
    }

    public async Task<Result<CartResponse>> RemoveItemsAsync(RemoveCartItemsRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CartId))
            return Result<CartResponse>.Failure(CartErrors.InvalidCartId);

        if (request.VariantIds == null || !request.VariantIds.Any())
            return Result<CartResponse>.Failure(CartErrors.EmptyCart);

        var existingCart = await _cartRepository.GetCartAsync(request.CartId);
        if (existingCart is null)
            return Result<CartResponse>.Failure(CartErrors.NotFoundCart);

        var keys = request.VariantIds.ToHashSet();
        var notFound = request.VariantIds
            .Where(vId => !existingCart.Items.Any(i => i.VariantId == vId))
            .ToList();
        if (notFound.Any())
            return Result<CartResponse>.Failure(CartErrors.InvalidItemsIds);

        existingCart.Items = existingCart.Items
            .Where(i => !keys.Contains(i.VariantId))
            .ToList();

        var updatedCart = await _cartRepository.UpdateCartAsync(existingCart);

        return updatedCart is not null
            ? Result<CartResponse>.Success(updatedCart.Adapt<CartResponse>())
            : Result<CartResponse>.Failure(CartErrors.CantUpdateCart);
    }

    public async Task<Result<CartResponse>> AddItemsAsync(AddCartItemsRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CartId))
            return Result<CartResponse>.Failure(CartErrors.InvalidCartId);

        if (request.Items == null || !request.Items.Any())
            return Result<CartResponse>.Failure(CartErrors.EmptyCart);

        var cart = await _cartRepository.GetCartAsync(request.CartId);
        if (cart == null)
            return Result<CartResponse>.Failure(CartErrors.NotFoundCart);

        foreach (var itemRequest in request.Items)
        {
            CartItem? resolved = await ResolveCartItemAsync(itemRequest.VariantId, itemRequest.ProductId, itemRequest.Quantity);
            if (resolved == null)
                return Result<CartResponse>.Failure(ProductErrors.NotFoundProduct);

            var existingItem = cart.Items.FirstOrDefault(i => i.VariantId == itemRequest.VariantId);
            if (existingItem != null)
                existingItem.Quantity += itemRequest.Quantity;
            else
                cart.Items.Add(resolved);
        }

        var updatedCart = await _cartRepository.UpdateCartAsync(cart);

        return updatedCart != null
            ? Result<CartResponse>.Success(updatedCart.Adapt<CartResponse>())
            : Result<CartResponse>.Failure(CartErrors.CantUpdateCart);
    }

    public async Task<Result> DeleteAsync(string key)
    {
        var result = await _cartRepository.DeleteCartAsync(key);
        return result ? Result.Success() : Result.Failure(CartErrors.NotFoundCart);
    }

    private async Task<CartItem?> ResolveCartItemAsync(int variantId, int productId, int quantity)
    {
        var variant = await _unitOfWork.Repository<ProductVariant>()
            .GetByIdWithSpecAsync(new ProductVariantByIdWithProductSpec(variantId));
        if (variant == null || variant.ProductId != productId) return null;

        var product = variant.Product;
        string pictureUrl = string.IsNullOrEmpty(product.PictureUrl)
            ? ""
            : $"{_baseUrl}/{product.PictureUrl.TrimStart('/')}";

        return new CartItem
        {
            VariantId = variant.Id,
            ProductId = product.Id,
            Size = variant.Size,
            Color = variant.Color,
            Quantity = quantity,
            Price = variant.Price,
            ProductName = product.Name,
            Brand = product.Brand?.Name ?? "",
            Type = product.Category?.Name ?? "",
            PictureUrl = pictureUrl,
            Description = product.Description ?? ""
        };
    }
}
