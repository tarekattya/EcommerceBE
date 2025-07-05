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

        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
     
    }
}
