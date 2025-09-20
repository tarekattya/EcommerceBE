using Ecommerce.Core.Entites;
using Ecommerce.Core.Specifications;
using Ecommerce.Shared.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.RepositoryContracts
{
    public interface IGenericRepository<T> where T : AuditLogging
    {

        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);

        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec);
        Task<T?> GetByIdWithSpecAsync(ISpecification<T> spec);

        Task<int> GetCountAsync(ISpecification<T> spec);

    }
}
