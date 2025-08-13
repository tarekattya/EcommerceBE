using Ecommerce.Core.Abstraction;
using Ecommerce.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.RepositoryContracts
{
    public interface IGenericRepository<T> where T : AuditLogging
    {

        Task<Result<IEnumerable<T>>> GetAllAsync();
        Task<Result<T?>> GetByIdAsync(int id);
     
    }
}
