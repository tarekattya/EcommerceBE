using Ecommerce.Application.Services.Service.Contarct;
using Ecommerce.Core.Entites.ProductModule;
using Ecommerce.Core.RepositoryContracts;
using Ecommerce.Shared.Abstraction;
using Ecommerce.Shared.Abstraction.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Service.Implement
{
    public class BrandService(IGenericRepository<ProductBrand> Pbrepository) : IBrandService
    {
        private readonly IGenericRepository<ProductBrand> _pbrepository = Pbrepository;

        public async Task<Result<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var result = await _pbrepository.GetAllAsync();
            if (result is null)
                return Result<IReadOnlyList<ProductBrand>>.Failure(ProductErrors.NotFoundBrand);
            return Result<IReadOnlyList<ProductBrand>>.Success(result);
        }

    }
}
