using Ecommerce.API.Mapping.ProductMap;
using Ecommerce.Application.Helper;
using Ecommerce.Core.Entites;
using Ecommerce.Core.Entites.ProductModule;
using Ecommerce.Core.Specifications.ProductSpecs;
using Ecommerce.Shared.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using productResponse = Ecommerce.Application.Helper.Dtos.Product.productResponse;

namespace Ecommerce.Application.Services.Service.Contarct
{
    public interface IProductService
    {
        public Task<Result<Pagination<productResponse>>> GetAllAsync(ProductSpecParams specParams);

        public Task<Result<productResponse>> GetProductById(int id);

        public Task<Result<IReadOnlyList<ProductBrand>>> GetBrands();

        public Task<Result<IReadOnlyList<ProductCategory>>> GetCategories();



    }
}
