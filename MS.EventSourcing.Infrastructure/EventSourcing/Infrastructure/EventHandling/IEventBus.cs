using System.Collections.Generic;

namespace MS.EventSourcing.Infrastructure.EventHandling
{
    /// <summary>
    /// Represents an event bus responsible for publishing events to event handlers
    /// </summary>
    public interface IEventBus
    {
        void PublishEvent(DomainEvent domainEvent);
        void PublishEvents(IEnumerable<DomainEvent> domainEvents);
    }
}
