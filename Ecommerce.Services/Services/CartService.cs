namespace Ecommerce.Application;

public class CartService(ICartRepository cartRepository, IUnitOfWork unitOfWork) : ICartService
{
    private readonly ICartRepository _cartRepository = cartRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

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

        List<int>? productIds = cartRequest.Items.Select(i => i.Id).ToList();
        IReadOnlyList<Product>? products = await _unitOfWork.Repository<Product>()
            .GetAllWithSpecAsync(new ProductsByIdsSpec(productIds));

        foreach (CartItemsRequest itemRequest in cartRequest.Items)
        {
            Product? product = products.FirstOrDefault(p => p.Id == itemRequest.Id);
            if (product is null)
                return Result<CartResponse>.Failure(ProductErrors.NotFoundProduct);

            CartItem? cartItem = product.Adapt<CartItem>();
            cartItem.Quantity = itemRequest.Quantity;
            cart.Items.Add(cartItem);
        }

        cart.Items = cart.Items
            .GroupBy(i => i.Id)
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

        var existingItemIds = cart.Items.Select(i => i.Id).ToList();
        var requestItemIds = cartRequest.Items.Select(i => i.Id).ToList();
        var intersectIds = existingItemIds.Intersect(requestItemIds).ToList();

        if (!intersectIds.Any())
            return Result<CartResponse>.Failure(CartErrors.InvalidItemsIds);

        var products = await _unitOfWork.Repository<Product>()
            .GetAllWithSpecAsync(new ProductsByIdsSpec(intersectIds));

        foreach (var item in cartRequest.Items)
        {
            var existingItem = cart.Items.FirstOrDefault(i => i.Id == item.Id);
            if (existingItem is null)
                continue; 

            var product = products.FirstOrDefault(p => p.Id == item.Id);
            if (product is null)
                return Result<CartResponse>.Failure(ProductErrors.NotFoundProduct);

            existingItem.Quantity = item.Quantity;
            existingItem.Price = product.Price;
            existingItem.ProductName = product.Name;
            existingItem.Brand = product.Brand.Name;
            existingItem.Type = product.Category.Name;
            existingItem.PictureUrl = product.PictureUrl;
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

        if (request.ItemIds == null || !request.ItemIds.Any())
            return Result<CartResponse>.Failure(CartErrors.EmptyCart);

        var existingCart = await _cartRepository.GetCartAsync(request.CartId);
        if (existingCart is null)
            return Result<CartResponse>.Failure(CartErrors.NotFoundCart);

        var notFoundItems = request.ItemIds
        .Where(id => !existingCart.Items.Any(i => i.Id == id))
        .ToList();

        if (notFoundItems.Any())
        {
            return Result<CartResponse>.Failure(CartErrors.InvalidItemsIds);
        }
        existingCart.Items = existingCart.Items
            .Where(i => !request.ItemIds.Contains(i.Id))
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

        var productIds = request.Items.Select(x => x.Id).ToList();
        var products = await _unitOfWork.Repository<Product>()
            .GetAllWithSpecAsync(new ProductsByIdsSpec(productIds));

        foreach (var itemRequest in request.Items)
        {
            var product = products.FirstOrDefault(p => p.Id == itemRequest.Id);
            if (product == null)
                return Result<CartResponse>.Failure(ProductErrors.NotFoundProduct);

            var existingItem = cart.Items.FirstOrDefault(i => i.Id == itemRequest.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += itemRequest.Quantity;
            }
            else
            {
                var newItem = itemRequest.Adapt<CartItem>();
                newItem.ProductName = product.Name;
                newItem.Price = product.Price;
                newItem.Brand = product.Brand.Name;
                newItem.Type = product.Category.Name;
                newItem.PictureUrl = product.PictureUrl;

                cart.Items.Add(newItem);
            }
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

}
