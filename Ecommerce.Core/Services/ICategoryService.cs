namespace Ecommerce.Core;

public interface ICategoryService
{
    public Task<Result<Pagination<CategoryResponse>>> GetCategories(CategorySpecParams specParams);

    public Task<Result<CategoryResponse>> GetCategoryById(int id);

    public Task<Result<CategoryResponse>> CreateCategory(CategoryRequest category);

    public Task<Result> UpdateCategory(int id, CategoryRequest category);

    public Task<Result> DeleteCategory(int id);



}
