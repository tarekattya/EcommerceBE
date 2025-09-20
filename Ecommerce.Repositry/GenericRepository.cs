
using Ecommerce.Core.Entites;
using Ecommerce.Core.RepositoryContracts;
using Ecommerce.Core.Specifications;
using  Ecommerce.Infrastructure.Data;
using Ecommerce.Shared.Abstraction;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure
{
    public class GenericRepository<T>(ApplicationDbContext dbContext) : IGenericRepository<T> where T : AuditLogging
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }
        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)
        {
            var data = await ApplaySpecifications(spec)
                .AsNoTracking()
                .ToListAsync();
            if (data is null || !data.Any())
                return null!;

            
            return data;

        }
        public async Task<T?> GetByIdWithSpecAsync(ISpecification<T> spec)
        {
            var singledata = await ApplaySpecifications(spec)
                .FirstOrDefaultAsync();
            if (singledata is null)
                return null!;
            return singledata;


        }
        public async Task<int> GetCountAsync(ISpecification<T> spec)
        {
            return await ApplaySpecifications(spec).CountAsync();
        }


        private IQueryable<T> ApplaySpecifications(ISpecification<T> spec) => 
            SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec);

        
        
    }
}
