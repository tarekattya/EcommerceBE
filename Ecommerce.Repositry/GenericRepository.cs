using Ecommerce.Core.Entites;
using Ecommerce.Core.RepositoryContracts;
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

        public async Task<IEnumerable<T>> GetAllAsync() {
            
            if(typeof(T) == typeof(Product))
                return (IEnumerable<T>) await _dbContext.Set<Product>().Include(P=>P.Brand).Include(P=>P.Category).AsNoTracking().ToListAsync();



            return await _dbContext.Set<T>().AsNoTracking().ToListAsync(); }
        public async Task<T?> GetByIdAsync(int id)
        {
            if(typeof(T) == typeof(Product))
                return await _dbContext.Set<Product>().Include(P => P.Brand).Include(P => P.Category).AsNoTracking().FirstOrDefaultAsync(p => p.Id == id) as T;
            return await _dbContext.Set<T>().FindAsync(id);
        }
    }
}
