using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MS.Infrastructure;

namespace MS.EventSourcing.Infrastructure.EventHandling
{
    /// <summary>
    /// In memory event store used for testing purposes
    /// </summary>
    public class InMemoryEventStore : IEventStore
    {
        private static readonly ConcurrentDictionary<Uuid, List<DomainEvent>> Events = new ConcurrentDictionary<Uuid, List<DomainEvent>>();

        public IEnumerable<DomainEvent> GetEvents(Uuid aggregateRootId, string aggregateType, long startVersion)
        {
            return Events.ContainsKey(aggregateRootId)
                ? Events[aggregateRootId].Where(e => e.Sequence >= startVersion)
                : new List<DomainEvent>();
        }

        public void Insert(Uuid aggregateRootId, string aggregateType, IEnumerable<DomainEvent> domainEvents)
        {
            var events = domainEvents.ToList();

            if (!events.Any())
            {
                return;
            }

            var queue = new List<DomainEvent>();

            if (Events.ContainsKey(aggregateRootId))
            {
                queue = Events[aggregateRootId];
            }
            else
            {
                Events[aggregateRootId] = queue;                
            }

            queue.AddRange(events);
        }

        public void Clear()
        {
            Events.Clear();
        }
    }
}
