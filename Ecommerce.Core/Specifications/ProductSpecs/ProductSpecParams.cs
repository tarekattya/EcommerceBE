
namespace Ecommerce.Core;

    public record ProductSpecParams
    {
        private const int MaxPageSize = 10;
        private int? _pageSize;
        private int? _pageIndex;
        private string? _search;

        public string? Sort { get; init; }
        public int? BrandId { get; init; }
        public int? CategoryId { get; init; }

        public int pageIndex
        {
            get => _pageIndex.HasValue && _pageIndex.Value > 0 ? _pageIndex.Value : 1;
            init => _pageIndex = value;
        }

        public int PageSize
        {
            get => _pageSize.HasValue && _pageSize.Value > 0 ? Math.Min(_pageSize.Value, MaxPageSize) : MaxPageSize;
            init => _pageSize = value;
        }

        public string? Search
        {
            get => _search;
            init => _search = value?.ToLower();
        }
    }




