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

    public class ProductController(IGenericRepository<Product> repository) : ApiBaseController
    {
        private readonly IGenericRepository<Product> _repository = repository;

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<productResponse>>> GetAllProducts()
        {
            var spec = new ProductSpecWithBrandAndCategory();
            var result = await _repository.GetAllWithSpecAsync(spec);

            if (!result.IsSuccess)
                return NotFound(ProductErrors.NotFoundProduct);

            var response = result.Value.Adapt<IEnumerable<productResponse>>();
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
    }
}
