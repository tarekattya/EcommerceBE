using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Specifications.ProductSpecs
{
    public record ProductSpecParams
    {
        private const int MaxPageSize = 10;
        private int? _pageSize;
        private int? _pageIndex;
        private string? _search;

        public string? Sort { get; init; }
        public int? BrandId { get; init; }
        public int? CategoryId { get; init; }

        // PageIndex مع default 1
        public int pageIndex
        {
            get => _pageIndex.HasValue && _pageIndex.Value > 0 ? _pageIndex.Value : 1;
            init => _pageIndex = value;
        }

        // PageSize مع default 10 و MaxPageSize
        public int PageSize
        {
            get => _pageSize.HasValue && _pageSize.Value > 0 ? Math.Min(_pageSize.Value, MaxPageSize) : MaxPageSize;
            init => _pageSize = value;
        }

        // Search يحول كل شيء لـ lowercase
        public string? Search
        {
            get => _search;
            init => _search = value?.ToLower();
        }
    }



}
