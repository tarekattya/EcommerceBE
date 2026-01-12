using System.Collections.Generic;

namespace Ecommerce.Core;

public interface IDomainEvent { }

public abstract class BaseDomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
