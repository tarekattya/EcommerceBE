using Ecommerce.Shared;

namespace Ecommerce.Core;

    public interface IBrandService
    {
        public Task<Result<IReadOnlyList<BrandResponse>>> GetBrands();

        public Task<Result<BrandResponse>> GetBrandById(int id);

        public Task<Result<BrandResponse>> CreateBrand(BrandRequest brand);

        public Task<Result> UpdateBrand(int id, BrandRequest brand);
        public Task<Result> DeleteBrand(int id);

    }

