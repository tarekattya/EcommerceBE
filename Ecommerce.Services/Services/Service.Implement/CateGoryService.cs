namespace Ecommerce.Application;

public class CategoryService(IGenericRepository<ProductCategory> pcrepository , IGenericRepository<Product> prepository) : ICategoryService
{
    private readonly IGenericRepository<ProductCategory> _pcrepository = pcrepository;
    private readonly IGenericRepository<Product> _prepository = prepository;
    public async Task<Result<IReadOnlyList<CategoryResponse>>> GetCategories()
    {
        var result = await _pcrepository.GetAllAsync();
        if (result is null)
            return Result<IReadOnlyList<CategoryResponse>>.Failure(CategoryErrors.NotFoundCate);
        return Result<IReadOnlyList<CategoryResponse>>.Success(result.Adapt<IReadOnlyList<CategoryResponse>>());
    }
    public async Task<Result<CategoryResponse>> GetCategoryById(int id)
    {
        var result = await _pcrepository.GetByIdAsync(id);
        if (result is null)
            return Result<CategoryResponse>.Failure(CategoryErrors.NotFoundCate);
        return Result<CategoryResponse>.Success(result.Adapt<CategoryResponse>());
    }
    public async Task<Result<CategoryResponse>> CreateCategory(CategoryRequest category)
    {
        var exists = await _pcrepository.GetCountAsync(new CategoryWithNameSpec(category.Name));
        if (exists > 0)
            return Result<CategoryResponse>.Failure(CategoryErrors.CategoryNameAlreadyExists);
        var newCategory = category.Adapt<ProductCategory>();
        var result = await _pcrepository.AddAsync(newCategory);
        if (result is null)
            return Result<CategoryResponse>.Failure(CategoryErrors.CantCreateCategory);
        return Result<CategoryResponse>.Success(result.Adapt<CategoryResponse>());
    }
    public async Task<Result> UpdateCategory(int id, CategoryRequest category)
    {
        var exists = await _pcrepository.GetCountAsync(new CategoryWithNameSpec(category.Name));
        if (exists > 0)
            return Result<BrandResponse>.Failure(CategoryErrors.CategoryNameAlreadyExists);
        var existingCategory = await _pcrepository.GetByIdAsync(id);
        if (existingCategory is null)
            return Result.Failure(CategoryErrors.NotFoundCate);
        existingCategory.Name = category.Name;
        await _pcrepository.UpdateAsync(existingCategory);
        return Result.Success();
    }
    public async Task<Result> DeleteCategory(int id)
    {
        var existingCategory = await _pcrepository.GetByIdAsync(id);
        if (existingCategory is null)
            return Result.Failure(CategoryErrors.NotFoundCate);
        if (existingCategory is not null && await _prepository.GetCountAsync(new ProductsByCateSpec(id)) > 0)
            return Result.Failure(CategoryErrors.CateHasProducts);
       await _pcrepository.DeleteAsync(existingCategory!);
        return Result.Success();
    }
}
