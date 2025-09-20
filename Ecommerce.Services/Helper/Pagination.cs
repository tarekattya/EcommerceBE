namespace Ecommerce.Application.Helper
{
    public record Pagination<T>(int page, int pageSize, int count, IReadOnlyList<T> Data);
    
    
    
}
