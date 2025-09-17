using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Specifications.ProductSpecs
{
    public record ProductSpecParams(string? Sort, int? BrandId, int? CategoryId , int pageIndex)
    {
		private const int MaxPgaeSize = 10;
		private int pageSize;

		public int PageSize
        {
			get { return pageSize; }
			set { pageSize = value > MaxPgaeSize ? MaxPgaeSize : value; }
		}

		private string search;

		public string Search
        {
			get { return  search; }
			set {  search = value.ToLower(); }
		}



	}
    


}
