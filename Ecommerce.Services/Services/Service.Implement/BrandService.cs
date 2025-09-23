using Ecommerce.Shared.Helper.Dtos.Brand;
using Ecommerce.Core.Services.Service.Contarct;
using Ecommerce.Core.Specifications.BranSpecs;
using Ecommerce.Core.Specifications.ProductSpecs;


namespace Ecommerce.Application.Services.Service.Implement
{
    public class BrandService(IGenericRepository<ProductBrand> Pbrepository , IGenericRepository<Product> prepository) : IBrandService
    {
        private readonly IGenericRepository<ProductBrand> _pbrepository = Pbrepository;
        private readonly IGenericRepository<Product> _prepository = prepository;


        public async Task<Result<IReadOnlyList<BrandResponse>>> GetBrands()
        {
            var result = await _pbrepository.GetAllAsync();
            if (result is null)
                return Result<IReadOnlyList<BrandResponse>>.Failure(ProductErrors.NotFoundBrand);

            return Result<IReadOnlyList<BrandResponse>>.Success(result.Adapt<IReadOnlyList<BrandResponse>>());
        }


        public async Task<Result<BrandResponse>> GetBrandById(int id)
        {
            var barnd = await _pbrepository.GetByIdAsync(id);
            if(barnd is null)
                return Result<BrandResponse>.Failure(ProductErrors.NotFoundBrand);
            return Result<BrandResponse>.Success(barnd.Adapt<BrandResponse>());
        }
        public async Task<Result<BrandResponse>> CreateBrand(BrandRequest brand)
        {
            var exists = await _pbrepository.GetCountAsync(new BrandsByNameSpec(brand.Name));
            if (exists > 0)
                return Result<BrandResponse>.Failure(ProductErrors.BrandNameAlreadyExists);

            var newBrand = brand.Adapt<ProductBrand>();
            var createdBrand = await _pbrepository.AddAsync(newBrand);
            return Result<BrandResponse>.Success(createdBrand.Adapt<BrandResponse>());

        }
        public async Task<Result> UpdateBrand(int id , BrandRequest brand)
        {

            var exists = await _pbrepository.GetCountAsync(new BrandsByNameSpec(brand.Name));
            if (exists > 0)
                return Result<BrandResponse>.Failure(ProductErrors.BrandNameAlreadyExists);
            var existingBrand = await  _pbrepository.GetByIdAsync(id);
            if (existingBrand is null)
                return Result.Failure(ProductErrors.NotFoundBrand);
            existingBrand.Name = brand.Name;
            await _pbrepository.UpdateAsync(existingBrand);
            return Result.Success();

        }
        public async Task<Result> DeleteBrand(int id)
        {
            var existingBrand = await _pbrepository.GetByIdAsync(id);
            if (existingBrand is null)
                return Result.Failure(ProductErrors.NotFoundBrand);
            var Forcount = new ProductsByBrandSpec(id);
            var IsHasProduct = await _prepository.GetCountAsync(Forcount);

            if(IsHasProduct > 0)
                return Result.Failure(ProductErrors.BrandHasProducts);

            await _pbrepository.DeleteAsync(existingBrand);
            return Result.Success();



        }
    }
}
