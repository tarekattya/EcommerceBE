
using Ecommerce.Shared;

namespace Ecommerce.Application;

public class ProductService(IUnitOfWork unitOfWork, ICacheService cacheService) : IProductService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result<Pagination<productResponse>>> GetAllAsync(ProductSpecParams specParams)
    {
        var spec = new ProductSpecWithBrandAndCategory(specParams);
        var result = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);

        if (result is null)
            return Result<Pagination<productResponse>>.Failure(ProductErrors.InValidInputs);
        var Forcount = new ProductWithSpecificationFilterionForCount(specParams);
        var count = await _unitOfWork.Repository<Product>().GetCountAsync(Forcount);

        var response = result.Adapt<IReadOnlyList<productResponse>>();

        return Result<Pagination<productResponse>>.Success(new Pagination<productResponse>(specParams.pageIndex, specParams.PageSize, count, response));
    }
    public async Task<Result<productResponse>> GetProductById(int id)
    {

        var Spec = new ProductSpecWithBrandAndCategory(id);

        var result = await _unitOfWork.Repository<Product>().GetByIdWithSpecAsync(Spec);

        var product = result.Adapt<productResponse>();

        return product is not null ? Result<productResponse>.Success(product) : Result<productResponse>.Failure(ProductErrors.NotFoundProduct);

    }
    public async Task<Result<productResponse>> CreateProduct(ProductRequest product)
    {
        var exists = await _unitOfWork.Repository<Product>().GetCountAsync(new ProductsByNameSpec(product.Name));
        if (exists > 0)
            return Result<productResponse>.Failure(ProductErrors.ProductNameAlreadyExists);
        ProductCategory? category = await _unitOfWork.Repository<ProductCategory>().GetByIdAsync(product.CategoryId);
        if (category is null)
            return Result<productResponse>.Failure(CategoryErrors.NotFoundCate);

        ProductBrand? brand = await _unitOfWork.Repository<ProductBrand>().GetByIdAsync(product.BrandId);
        if (brand is null)
            return Result<productResponse>.Failure(BrandErrors.NotFoundBrand);

        Product? newProduct = new Product(
            product.Name,
            product.Description,
            product.PictureUrl,
            product.Price,
            product.BrandId,
            product.CategoryId,
            product.Stock
        );
        var createdProduct = await _unitOfWork.Repository<Product>().AddAsync(newProduct);
        await _unitOfWork.CompleteAsync();

        Result<productResponse>? result = await GetProductById(createdProduct.Id);

        await _cacheService.RemoveCacheByPrefixAsync("/api/products");

        return Result<productResponse>.Success(result.Value);

    }

    public async Task<Result<productResponse>> UpdateProduct(int id, ProductRequest request)
    {
       
        int exists = await _unitOfWork.Repository<Product>().GetCountAsync(new ProductsByNameAndIdSpec(request.Name , id));
        if (exists > 0)
            return Result<productResponse>.Failure(ProductErrors.ProductNameAlreadyExists);
        ProductSpecWithBrandAndCategory? Spec = new ProductSpecWithBrandAndCategory(id);
        Product? product = await _unitOfWork.Repository<Product>().GetByIdWithSpecAsync(Spec);
        if (product is not null)
        {
            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.BrandId = request.BrandId;
            product.CategoryId = request.CategoryId;
            product.PictureUrl = request.PictureUrl;
            
            product.UpdateStock(request.Stock);

            _unitOfWork.Repository<Product>().Update(product);
            await _unitOfWork.CompleteAsync();

            await _cacheService.RemoveCacheByPrefixAsync("/api/products");

            return Result<productResponse>.Success(product.Adapt<productResponse>());

        }
        return Result<productResponse>.Failure(ProductErrors.NotFoundProduct);

    }
    public async Task<Result> DeleteProduct(int id)
    {
        ProductSpecWithBrandAndCategory? Spec = new ProductSpecWithBrandAndCategory(id);

        Product? result = await _unitOfWork.Repository<Product>().GetByIdWithSpecAsync(Spec);

        if (result is null)
            return Result<bool>.Failure(ProductErrors.NotFoundProduct);

        _unitOfWork.Repository<Product>().Delete(result);
        await _unitOfWork.CompleteAsync();

        await _cacheService.RemoveCacheByPrefixAsync("/api/products");

        return Result.Success();
    }

    public async Task<Result<ProductFiltersResponse>> GetProductFiltersAsync()
    {
        var products = await _unitOfWork.Repository<Product>().GetAllAsync();
        var brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
        var categories = await _unitOfWork.Repository<ProductCategory>().GetAllAsync();

        var brandFilters = brands.Select(b => new FilterItemResponse(
            b.Id,
            b.Name,
            products.Count(p => p.BrandId == b.Id)
        )).ToList();

        var categoryFilters = categories.Select(c => new FilterItemResponse(
            c.Id,
            c.Name,
            products.Count(p => p.CategoryId == c.Id)
        )).ToList();

        var maxPrice = products.Any() ? products.Max(p => p.Price) : 0;

        return Result<ProductFiltersResponse>.Success(new ProductFiltersResponse(brandFilters, categoryFilters, maxPrice));
    }
}

