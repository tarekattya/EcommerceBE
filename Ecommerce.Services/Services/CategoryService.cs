namespace Ecommerce.Application;

public class CategoryService(IUnitOfWork unitOfWork) : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<IReadOnlyList<CategoryResponse>>> GetCategories()
    {
        var result = await _unitOfWork.Repository<ProductCategory>().GetAllAsync();
        if (result is null)
            return Result<IReadOnlyList<CategoryResponse>>.Failure(CategoryErrors.NotFoundCate);
        return Result<IReadOnlyList<CategoryResponse>>.Success(result.Adapt<IReadOnlyList<CategoryResponse>>());
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
        return Result.Success();
    }
}
