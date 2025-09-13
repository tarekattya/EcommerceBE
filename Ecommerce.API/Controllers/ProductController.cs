using Ecommerce.API.Mapping.ProductMap;
using Ecommerce.Core.Abstraction.Errors;
using Ecommerce.Core.Entites;
using Ecommerce.Core.RepositoryContracts;
using Ecommerce.Core.Specifications.ProductSpecs;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{

    public class ProductController(
        
        IGenericRepository<Product> Productrepository,
                IGenericRepository<ProductBrand> Brandrepository,
                IGenericRepository<ProductCategory> Caterepository




        ) : ApiBaseController
    {
        private readonly IGenericRepository<Product> _repository = Productrepository;
        private readonly IGenericRepository<ProductBrand> _brandrepository = Brandrepository;
        private readonly IGenericRepository<ProductCategory> _caterepository = Caterepository;

        [HttpGet("")]
        public async Task<ActionResult<IReadOnlyList<productResponse>>> GetAllProducts(string? sort , int? brandId , int? categoryId  )
        {
            var spec = new ProductSpecWithBrandAndCategory(sort , brandId , categoryId);
            var result = await _repository.GetAllWithSpecAsync(spec);

            if (!result.IsSuccess)
                return NotFound(ProductErrors.InValidInputs);

            var response = result.Value.Adapt<IReadOnlyList<productResponse>>();
            return Ok(response);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<productResponse>> GetProductById([FromRoute] int id)
        {
            var Spec = new ProductSpecWithBrandAndCategory(id);

            var result = await _repository.GetByIdWithSpecAsync(Spec);

            var product = result.Value.Adapt<productResponse>();
            return product is not null ? Ok(product) : NotFound(ProductErrors.NotFoundProduct);

           
        }

        [HttpGet("brands")]
        public async Task<ActionResult<ProductBrand>> GetBrands()
        {
            var brands = await _brandrepository.GetAllAsync();
            return brands.IsSuccess ? Ok(brands.Value) : NotFound(ProductErrors.NotFoundBrand);

        }

        [HttpGet("categories")]
        public async Task<ActionResult<ProductCategory>> GetCategories()
        {
            var brands = await _caterepository.GetAllAsync();
            return brands.IsSuccess ? Ok(brands.Value) : NotFound(ProductErrors.NotFoundCate);

        }

    }
}
