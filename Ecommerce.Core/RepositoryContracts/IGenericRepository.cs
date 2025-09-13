using Ecommerce.Core.Abstraction;
using Ecommerce.Core.Entites;
using Ecommerce.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.RepositoryContracts
{
    public interface IGenericRepository<T> where T : AuditLogging
    {

        Task<Result<IReadOnlyList<T>>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);

        Task<Result<IReadOnlyList<T>>> GetAllWithSpecAsync(ISpecification<T> spec);
        Task<Result<T?>> GetByIdWithSpecAsync(ISpecification<T> spec);

    }
}
