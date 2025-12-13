namespace Ecommerce.Infrastructure;

public class GenericRepository<T>(ApplicationDbContext dbContext) : IGenericRepository<T> where T : BaseEntity
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

    public async Task<T> AddAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
         _dbContext.Set<T>().Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public Task DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        return _dbContext.SaveChangesAsync();
    }
}
