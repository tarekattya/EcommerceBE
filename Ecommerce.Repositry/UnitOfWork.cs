
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce;

public class UnitOfWork : IUnitOfWork
{
    Dictionary<string, object> repositories;
    private readonly ApplicationDbContext _context;
    private readonly IServiceProvider _serviceProvider;

    public UnitOfWork(ApplicationDbContext context, IServiceProvider serviceProvider)
    {
        _context = context;
        _serviceProvider = serviceProvider;
        repositories = new Dictionary<string, object>();
    }

    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
    {
        string key = typeof(TEntity).Name;

        if (!repositories.ContainsKey(key))
        {
            var repository = new GenericRepository<TEntity>(_context);
            repositories.Add(key, repository);
        }

        return (repositories[key] as IGenericRepository<TEntity>)!;
    }

    public async Task<int> CompleteAsync()
    {
        int totalResult = await _context.SaveChangesAsync();
        await DispatchDomainEvents();
        return totalResult;
    }

    private async Task DispatchDomainEvents()
    {
        while (true)
        {
            var domainEntities = _context.ChangeTracker
                .Entries<BaseEntity>()
                .Where(x => x.Entity.DomainEvents.Any())
                .ToList();

            var hasChanges = _context.ChangeTracker.HasChanges();

            if (!domainEntities.Any() && !hasChanges) break;

            if (domainEntities.Any())
            {
                var domainEvents = domainEntities
                    .SelectMany(x => x.Entity.DomainEvents)
                    .ToList();

                domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

                foreach (var domainEvent in domainEvents)
                {
                    await DispatchEvent(domainEvent);
                }
            }

            // Only save if there are changes to avoid endless loops if handlers don't change anything
            if (_context.ChangeTracker.HasChanges())
            {
                await _context.SaveChangesAsync();
            }
            else if (!domainEntities.Any())
            {
                break;
            }
        }
    }

    private async Task DispatchEvent(IDomainEvent domainEvent)
    {
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handlers = _serviceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            if (handler != null)
            {
                var method = handlerType.GetMethod("Handle");
                if (method != null)
                {
                    await (Task)method.Invoke(handler, new object[] { domainEvent })!;
                }
            }
        }
    }

    public async ValueTask DisposeAsync()
    => await _context.DisposeAsync();

}   
