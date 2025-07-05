using Ecommerce.Core.Entites;
using Ecommerce.Core.RepositoryContracts;
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
            var products = await _repository.GetAllAsync();
            return Ok(products);
        }
    }
}
