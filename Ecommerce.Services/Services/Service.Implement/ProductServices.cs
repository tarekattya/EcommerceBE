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
    public class ProductService(IGenericRepository<Product> repository ,
        IGenericRepository<ProductCategory> categoryRepo,
        IGenericRepository<ProductBrand> brandRepo) : IProductService


    {
        private readonly IGenericRepository<Product> _Prepository = repository;
        private readonly IGenericRepository<ProductCategory> _categoryRepo = categoryRepo;
        private readonly IGenericRepository<ProductBrand> _brandRepo = brandRepo;



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
        public async Task<Result<productResponse>> CreateProduct(ProductRequest product)
        {
            var category = await _categoryRepo.GetByIdAsync(product.CategoryId);
            if (category is null)
                return Result<productResponse>.Failure(ProductErrors.NotFoundCate);

            var brand = await _brandRepo.GetByIdAsync(product.BrandId);
            if (brand is null)
                return Result<productResponse>.Failure(ProductErrors.NotFoundBrand);

            var newProduct = product.Adapt<Product>();
            var createdProduct = await _Prepository.AddAsync(newProduct);


           var result = await GetProductById(createdProduct.Id);

            return Result<productResponse>.Success(result.Value);





        }

        public async Task<Result<productResponse>> UpdateProduct(int id, ProductRequest request)
        {
            var Spec = new ProductSpecWithBrandAndCategory(id);

            var product = await _Prepository.GetByIdWithSpecAsync(Spec);

            product.Name = request.Name;
            product.Price = request.Price;
            product.BrandId = request.BrandId;
            product.CategoryId = request.CategoryId;
            product.PictureUrl = request.PictureUrl;

            await _Prepository.UpdateAsync(product);
            var result = await GetProductById(product.Id);
            return Result<productResponse>.Success(result.Value);

        }
        public async Task<Result<bool>> DeleteProduct(int id)
        {
            var Spec = new ProductSpecWithBrandAndCategory(id);

            var result = await _Prepository.GetByIdWithSpecAsync(Spec);

            if(result is null)
                return Result<bool>.Failure(ProductErrors.NotFoundProduct);

            await _Prepository.DeleteAsync(result);
            return Result<bool>.Success(true);
        }
    }
}
