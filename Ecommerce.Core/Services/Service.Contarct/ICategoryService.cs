namespace Ecommerce.Core;

public interface ICategoryService
{
    public Task<Result<IReadOnlyList<CategoryResponse>>> GetCategories();

    public Task<Result<CategoryResponse>> GetCategoryById(int id);

    public Task<Result<CategoryResponse>> CreateCategory(CategoryRequest category);

    public Task<Result> UpdateCategory(int id, CategoryRequest category);

    public Task<Result> DeleteCategory(int id);



}
