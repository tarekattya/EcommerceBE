using Ecommerce.Shared.Helper.Dtos.Product;
using Ecommerce.Core.Entites;
using Ecommerce.Core.Entites.ProductModule;
using Ecommerce.Core.Specifications.ProductSpecs;
using Ecommerce.Shared.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Shared.Helper;


namespace Ecommerce.Application.Services.Service.Contarct
{
    public interface IProductService
    {
        public Task<Result<Pagination<productResponse>>> GetAllAsync(ProductSpecParams specParams);

        public Task<Result<productResponse>> GetProductById(int id);


        public Task<Result<productResponse>> CreateProduct(ProductRequest product);

        public Task<Result<productResponse>> UpdateProduct(int id, ProductRequest request);

        public Task<Result> DeleteProduct(int id);

    }
}
