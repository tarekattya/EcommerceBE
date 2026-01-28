namespace Ecommerce.Core;

public record OrderSpecParams
{
    private const int MaxPageSize = 50;
    private int? _pageSize;
    private int? _pageIndex;
    private string? _search;

    public string? Sort { get; init; }
    public OrderStatus? Status { get; init; }
    public decimal? MinAmount { get; init; }
    public decimal? MaxAmount { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }

    public int PageIndex
    {
        get => _pageIndex.HasValue && _pageIndex.Value > 0 ? _pageIndex.Value : 1;
        init => _pageIndex = value;
    }

    public int PageSize
    {
        get => _pageSize.HasValue && _pageSize.Value > 0 ? Math.Min(_pageSize.Value, MaxPageSize) : 10;
        init => _pageSize = value;
    }

    public string? Search
    {
        get => _search;
        init => _search = value?.ToLower();
    }
}
