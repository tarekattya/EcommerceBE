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
    public class CateGoryService(IGenericRepository<ProductCategory> pcrepository) : ICateGoryService
    {
        private readonly IGenericRepository<ProductCategory> _pcrepository = pcrepository;

        public async Task<Result<IReadOnlyList<ProductCategory>>> GetCategories()
        {
            var result = await _pcrepository.GetAllAsync();
            if (result is null)
                return Result<IReadOnlyList<ProductCategory>>.Failure(ProductErrors.NotFoundCate);
            return Result<IReadOnlyList<ProductCategory>>.Success(result);

        }
    }
}
