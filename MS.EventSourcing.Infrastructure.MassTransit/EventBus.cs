using System.Collections.Generic;
using MS.EventSourcing.Infrastructure.EventHandling;

namespace MS.EventSourcing.Infrastructure.MassTransit
{
    public class EventBus : Bus, IEventBus
    {
        public void PublishEvent(DomainEvent domainEvent)
        {
            object @event = domainEvent;
            ServiceBus.Publish(@event);
        }

        public void PublishEvents(IEnumerable<DomainEvent> domainEvents)
        {
            foreach (var @event in domainEvents)
            {
                PublishEvent(@event);
            }
        }
    }
}
