using Ecommerce.API.Mapping.ProductMap;
using Ecommerce.Core.Abstraction.Errors;
using Ecommerce.Core.Entites;
using Ecommerce.Core.RepositoryContracts;
using Ecommerce.Core.Specifications.ProductSpecs;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{

    public class ProductController(IGenericRepository<Product> repository , IMapper mapper) : ApiBaseController
    {
        private readonly IGenericRepository<Product> _repository = repository;
        private readonly IMapper _mapper = mapper;

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<productResponse>>> GetAllProducts()
        {
            var spec = new ProductSpecWithBrandAndCategory();
            var result = await _repository.GetAllWithSpecAsync(spec);

            if (!result.IsSuccess)
                return BadRequest(ProductErrors.NotFoundProduct);

            var response = result.Value.Adapt<IEnumerable<productResponse>>();
            return Ok(response);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById([FromRoute] int id)
        {
            var Spec = new ProductSpecWithBrandAndCategory(id);

            var result = await _repository.GetByIdWithSpecAsync(Spec);
            
            return result.IsSuccess ? Ok(result.Value) : BadRequest(ProductErrors.NotFoundProduct); 
           
        }
    }
}
