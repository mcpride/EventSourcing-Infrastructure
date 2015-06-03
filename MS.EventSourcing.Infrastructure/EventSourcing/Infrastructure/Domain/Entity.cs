using MS.EventSourcing.Infrastructure.EventHandling;
using MS.Infrastructure;

namespace MS.EventSourcing.Infrastructure.Domain
{
    /// <summary>
    /// Represents an object contained within the aggregate root with its own identity
    /// and application of events
    /// </summary>
    public class Entity
    {
        public Entity(AggregateRoot parent, Uuid entityId)
        {
            Id = entityId;
            Parent = parent;
            if (Parent != null) Parent.Associate(this);
        }

        public void ApplyEvent(DomainEvent domainEvent)
        {
            var @event = domainEvent as DomainEntityEvent;
            if (@event != null)
            {
                @event.EntityId = Id;
            }
            Parent.ApplyEvent(domainEvent);
        }
        
        public Uuid Id { get; protected set; }

        public AggregateRoot Parent { get; set; }
    }
}
