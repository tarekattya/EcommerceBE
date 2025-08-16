using Ecommerce.Core.Abstraction.Errors;
using Ecommerce.Core.Entites;
using Ecommerce.Core.RepositoryContracts;
using Ecommerce.Core.Specifications.ProductSpecs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{

    public class ProductController(IGenericRepository<Product> repository) : ApiBaseController
    {
        private readonly IGenericRepository<Product> _repository = repository;

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            var Spec = new ProductSpecWithBrandAndCategory();
            var result = await _repository.GetAllWithSpecAsync(Spec);
            return result.IsSuccess ? Ok(result.Value) : BadRequest();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById([FromRoute] int id)
        {
            var Spec = new ProductSpecWithBrandAndCategory(id);

            var result = await _repository.GetByIdWithSpecAsync(Spec);
            
            return result.IsSuccess ? Ok(result.Value) : BadRequest(); 
           
        }
    }
}
