
namespace Ecommerce;

public class UnitOfWork : IUnitOfWork
{
    Dictionary<string, object> repositories;
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        repositories =new Dictionary<string , object>();
    }

    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
    {
        string key = typeof(TEntity).Name;

        if (!repositories.ContainsKey(key))
        {
            var repository = new GenericRepository<TEntity>(_context) ;
            repositories.Add(key, repository);
        }

        return repositories[key] as IGenericRepository<TEntity>;
    }

    public async Task<int> CompleteAsync()
    => await _context.SaveChangesAsync();
    

    public async ValueTask DisposeAsync()
    => await _context.DisposeAsync();

}   
