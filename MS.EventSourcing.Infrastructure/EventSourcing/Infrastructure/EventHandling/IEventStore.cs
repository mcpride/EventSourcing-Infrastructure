using System;
using System.Collections.Generic;
using MS.Infrastructure;

namespace MS.EventSourcing.Infrastructure.EventHandling
{
    /// <summary>
    /// Represents an event store for storing aggregate root event streams
    /// </summary>
    public interface IEventStore
    {
        IEnumerable<DomainEvent> GetEvents(Uuid aggregateRootId, string aggregateType, long startVersion);
        void Insert(Uuid aggregateRootId, string aggregateType, IEnumerable<DomainEvent> domainEvents);
    }
}