using Ecommerce.Shared.Helper.Dtos.Brand;



namespace Ecommerce.API.Controllers
{
    public class BrandsController(IBrandService service) : ApiBaseController
    {
        private readonly IBrandService _service = service;

        [HttpGet("")]
        public async Task<ActionResult<BrandResponse>> GetBrands()
        {
            var brands = await _service.GetBrands();
            return brands.IsSuccess ? Ok(brands.Value) : brands.ToProblem();

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BrandResponse>> GetBrandById([FromRoute] int id)
        {
            var brand = await _service.GetBrandById(id);
            return brand.IsSuccess ? Ok(brand.Value) : brand.ToProblem();
        }

        [HttpPost("")]
        public async Task<ActionResult<BrandResponse>> CreateBrand([FromQuery] BrandRequest request)
        {
            var brand = await _service.CreateBrand(request);
            return brand.IsSuccess ? Ok(brand.Value) : brand.ToProblem();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBrand([FromRoute] int id, [FromQuery] BrandRequest request)
        {
            var brand = await _service.UpdateBrand(id, request);
            return brand.IsSuccess ? NoContent() : brand.ToProblem();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBrand([FromRoute] int id)
        {
            var brand = await _service.DeleteBrand(id);
            return brand.IsSuccess ? NoContent() : brand.ToProblem();
        }

    }
}
