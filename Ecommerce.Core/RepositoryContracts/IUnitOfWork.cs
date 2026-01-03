namespace Ecommerce.Core;

public interface IUnitOfWork : IAsyncDisposable
{
    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;

    Task<int> CompleteAsync();

}
