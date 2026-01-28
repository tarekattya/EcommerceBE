namespace Ecommerce.Application;

public class CategoryService(IUnitOfWork unitOfWork, ICacheService cacheService) : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result<Pagination<CategoryResponse>>> GetCategories(CategorySpecParams specParams)
    {
        var spec = new CategoryWithSearchSpec(specParams);
        var countSpec = new CategoryCountSpec(specParams);

        var result = await _unitOfWork.Repository<ProductCategory>().GetAllWithSpecAsync(spec);
        var count = await _unitOfWork.Repository<ProductCategory>().GetCountAsync(countSpec);

        if (result is null)
            return Result<Pagination<CategoryResponse>>.Failure(CategoryErrors.NotFoundCate);

        var response = result.Adapt<IReadOnlyList<CategoryResponse>>();
        return Result<Pagination<CategoryResponse>>.Success(new Pagination<CategoryResponse>(specParams.PageIndex, specParams.PageSize, count, response));
    }
    public async Task<Result<CategoryResponse>> GetCategoryById(int id)
    {
        var result = await _unitOfWork.Repository<ProductCategory>().GetByIdAsync(id);
        if (result is null)
            return Result<CategoryResponse>.Failure(CategoryErrors.NotFoundCate);
        return Result<CategoryResponse>.Success(result.Adapt<CategoryResponse>());
    }
    public async Task<Result<CategoryResponse>> CreateCategory(CategoryRequest category)
    {
        var exists = await _unitOfWork.Repository<ProductCategory>().GetCountAsync(new CategoryWithNameSpec(category.Name));
        if (exists > 0)
            return Result<CategoryResponse>.Failure(CategoryErrors.CategoryNameAlreadyExists);
        var newCategory = category.Adapt<ProductCategory>();
        var result = await _unitOfWork.Repository<ProductCategory>().AddAsync(newCategory);
        if (result is null)
            return Result<CategoryResponse>.Failure(CategoryErrors.CantCreateCategory);
        await _unitOfWork.CompleteAsync();
        
        await _cacheService.RemoveCacheByPrefixAsync("/api/categories");

        return Result<CategoryResponse>.Success(result.Adapt<CategoryResponse>());
    }
    public async Task<Result> UpdateCategory(int id, CategoryRequest category)
    {
        var exists = await _unitOfWork.Repository<ProductCategory>().GetCountAsync(new CategoryWithNameSpec(category.Name));
        if (exists > 0)
            return Result<BrandResponse>.Failure(CategoryErrors.CategoryNameAlreadyExists);
        var existingCategory = await _unitOfWork.Repository<ProductCategory>().GetByIdAsync(id);
        if (existingCategory is null)
            return Result.Failure(CategoryErrors.NotFoundCate);
        existingCategory.Name = category.Name;
         _unitOfWork.Repository<ProductCategory>().Update(existingCategory);
       await _unitOfWork.CompleteAsync();

        await _cacheService.RemoveCacheByPrefixAsync("/api/categories");

        return Result.Success();
    }
    public async Task<Result> DeleteCategory(int id)
    {
        var existingCategory = await _unitOfWork.Repository<ProductCategory>().GetByIdAsync(id);
        if (existingCategory is null)
            return Result.Failure(CategoryErrors.NotFoundCate);
        if (existingCategory is not null && await _unitOfWork.Repository<Product>().GetCountAsync(new ProductsByCateSpec(id)) > 0)
            return Result.Failure(CategoryErrors.CateHasProducts);
        if(existingCategory is null)
            return Result.Failure(CategoryErrors.NotFoundCate);
        _unitOfWork.Repository <ProductCategory>().Delete(existingCategory);
        await _unitOfWork.CompleteAsync();

        await _cacheService.RemoveCacheByPrefixAsync("/api/categories");

        return Result.Success();
    }
}
