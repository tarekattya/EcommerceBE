using Ecommerce.Core.Entites.ProductModule;
using Ecommerce.Shared.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Service.Contarct
{
    public interface ICateGoryService
    {
        public Task<Result<IReadOnlyList<ProductCategory>>> GetCategories();

    }
}
