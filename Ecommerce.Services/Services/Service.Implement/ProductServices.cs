using Ecommerce.API.Mapping.ProductMap;
using Ecommerce.Application.Helper;
using Ecommerce.Application.Helper.Dtos.Product;
using Ecommerce.Application.Services.Service.Contarct;
using Ecommerce.Core.Entites;
using Ecommerce.Core.Entites.ProductModule;
using Ecommerce.Core.RepositoryContracts;
using Ecommerce.Core.Specifications.ProductSpecs;
using Ecommerce.Shared.Abstraction;
using Ecommerce.Shared.Abstraction.Errors;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Service.Implement
{
    public class ProductService(IGenericRepository<Product> repository, 
        IGenericRepository<ProductBrand> Pbrepository,
        IGenericRepository<ProductCategory> pcrepository) : IProductService


    {
        private readonly IGenericRepository<Product> _Prepository = repository;
        private readonly IGenericRepository<ProductBrand> _pbrepository = Pbrepository;
        private readonly IGenericRepository<ProductCategory> _pcrepository = pcrepository;

        public async Task<Result<Pagination<productResponse>>> GetAllAsync(ProductSpecParams specParams)
        {
            var spec = new ProductSpecWithBrandAndCategory(specParams);
            var result = await _Prepository.GetAllWithSpecAsync(spec);

            if (result is null)
                return Result<Pagination<productResponse>>.Failure(ProductErrors.InValidInputs);
            var Forcount = new ProductWithSpecificationFilterionForCount(specParams);
            var count = await _Prepository.GetCountAsync(Forcount);

            var response = result.Adapt<IReadOnlyList<productResponse>>();

            return Result<Pagination<productResponse>>.Success(new Pagination<productResponse>(specParams.pageIndex, specParams.PageSize, count, response));
        }
        public async Task<Result<productResponse>> GetProductById(int id)
        {
        
            var Spec = new ProductSpecWithBrandAndCategory(id);

            var result = await _Prepository.GetByIdWithSpecAsync(Spec);

            var product = result.Adapt<productResponse>();

            return product is not null ? Result<productResponse>.Success(product) : Result<productResponse>.Failure(ProductErrors.NotFoundProduct);

        }

        public async Task<Result<IReadOnlyList<ProductBrand>>> GetBrands()
        {
           var result = await _pbrepository.GetAllAsync();
            if (result is null)
                return Result<IReadOnlyList<ProductBrand>>.Failure(ProductErrors.NotFoundBrand);
            return Result<IReadOnlyList<ProductBrand>>.Success(result);
        }

        public async Task<Result<IReadOnlyList<ProductCategory>>> GetCategories()
        {
            var result = await _pcrepository.GetAllAsync();
            if (result is null)
                return Result<IReadOnlyList<ProductCategory>>.Failure(ProductErrors.NotFoundCate);
            return Result<IReadOnlyList<ProductCategory>>.Success(result);

        }

    }
}
