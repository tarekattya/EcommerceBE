using Ecommerce.Core.Abstraction;
using Ecommerce.Core.Abstraction.Errors;
using Ecommerce.Core.Entites;
using Ecommerce.Core.RepositoryContracts;
using Ecommerce.Core.Specifications;
using  Ecommerce.Infrastructure.Data;
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

        public async Task<Result<IEnumerable<T>>> GetAllAsync()
        {
            if (typeof(T) == typeof(Product))
            {
                var products = await _dbContext.Set<Product>()
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .AsNoTracking()
                    .ToListAsync();
                return Result<IEnumerable<T>>.Success(products.Cast<T>());
            }
            var data = await _dbContext.Set<T>().AsNoTracking().ToListAsync();
            return Result<IEnumerable<T>>.Success(data);
        }


        public async Task<T?> GetByIdAsync(int id)
        {
            if (typeof(T) == typeof(Product))
            {
                return await _dbContext.Set<Product>()
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id) as T;

            }
            return await _dbContext.Set<T>().FindAsync(id);
        }
        public async Task<Result<IEnumerable<T>>> GetAllWithSpecAsync(ISpecification<T> spec)
        {
            var data = await ApplaySpecifications(spec)
                .AsNoTracking()
                .ToListAsync();

            return  Result<IEnumerable<T>>.Success(data);

        }
        public async Task<Result<T?>> GetByIdWithSpecAsync(ISpecification<T> spec)
        {
            var singledata = await ApplaySpecifications(spec)
                .FirstOrDefaultAsync();

            
            return Result<T?>.Success(singledata);
        }

        private IQueryable<T> ApplaySpecifications(ISpecification<T> spec) => 
            SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec);



    }
}
