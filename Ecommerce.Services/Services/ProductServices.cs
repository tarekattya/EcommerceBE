
using Ecommerce.Shared;

namespace Ecommerce.Application;

public class ProductService(IUnitOfWork unitOfWork) : IProductService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

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
        var category = await _unitOfWork.Repository<ProductCategory>().GetByIdAsync(product.CategoryId);
        if (category is null)
            return Result<productResponse>.Failure(CategoryErrors.NotFoundCate);

        var brand = await _unitOfWork.Repository<ProductBrand>().GetByIdAsync(product.BrandId);
        if (brand is null)
            return Result<productResponse>.Failure(BrandErrors.NotFoundBrand);

        var newProduct = product.Adapt<Product>();
        var createdProduct = await _unitOfWork.Repository<Product>().AddAsync(newProduct);
        await _unitOfWork.CompleteAsync();

        var result = await GetProductById(createdProduct.Id);

        return Result<productResponse>.Success(result.Value);

    }

    public async Task<Result<productResponse>> UpdateProduct(int id, ProductRequest request)
    {
       
        var exists = await _unitOfWork.Repository<Product>().GetCountAsync(new ProductsByNameAndIdSpec(request.Name , id));
        if (exists > 0)
            return Result<productResponse>.Failure(ProductErrors.ProductNameAlreadyExists);
        var Spec = new ProductSpecWithBrandAndCategory(id);
        var product = await _unitOfWork.Repository<Product>().GetByIdWithSpecAsync(Spec);
        if (product is not null)
        {
            product.Name = request.Name;
            product.Price = request.Price;
            product.BrandId = request.BrandId;
            product.CategoryId = request.CategoryId;
            product.PictureUrl = request.PictureUrl;
            _unitOfWork.Repository<Product>().Update(product);
            await _unitOfWork.CompleteAsync();
            return Result<productResponse>.Success(product.Adapt<productResponse>());

        }
        return Result<productResponse>.Failure(ProductErrors.NotFoundProduct);

    }
    public async Task<Result> DeleteProduct(int id)
    {
        var Spec = new ProductSpecWithBrandAndCategory(id);

        var result = await _unitOfWork.Repository<Product>().GetByIdWithSpecAsync(Spec);

        if (result is null)
            return Result<bool>.Failure(ProductErrors.NotFoundProduct);

        _unitOfWork.Repository<Product>().Delete(result);
        await _unitOfWork.CompleteAsync();

        return Result.Success();
    }
}

