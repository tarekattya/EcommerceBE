using Ecommerce.Shared.Helper.Dtos.Brand;
using Ecommerce.Core.Entites.ProductModule;
using Ecommerce.Shared.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Services.Service.Contarct
{
    public interface IBrandService
    {
        public Task<Result<IReadOnlyList<BrandResponse>>> GetBrands();

        public Task<Result<BrandResponse>> GetBrandById(int id);

        public Task<Result<BrandResponse>> CreateBrand(BrandRequest brand);

        public Task<Result> UpdateBrand(int id, BrandRequest brand);
        public Task<Result> DeleteBrand(int id);

    }
}
