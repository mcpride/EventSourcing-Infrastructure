using System;
using System.Collections.Generic;
using MS.EventSourcing.Infrastructure.EventHandling;
using MS.Infrastructure;

namespace MS.EventSourcing.Infrastructure.Domain
{
    public interface IDomainRepository
    {
        /// <summary>
        /// Gets an aggregate root instance of type T for a given ID
        /// </summary>
        /// <typeparam name="T">Type of class to create and hydrate</typeparam>
        /// <param name="aggregateRootId">ID of the persisted aggregate root instance</param>
        /// <returns></returns>
        T GetById<T>(Uuid aggregateRootId) where T : AggregateRoot, new();

        /// <summary>
        /// Calls the generic GetSnapshot method of the snapshot store object.
        /// </summary>
        /// <param name="root">Snapshot-capable aggregate Root to load</param>
        /// <param name="aggregateRootId">ID of the persisted aggregate root instance</param>
        /// <returns>Snapshot of Type T casted as an object</returns>
        object GetSnapshot(AggregateRoot root, Uuid aggregateRootId);

        /// <summary>
        /// Upgrades outdated events to their current counterpart.
        /// </summary>
        /// <param name="events"></param>
        IEnumerable<DomainEvent> UpdateEventVersions(IEnumerable<DomainEvent> events);

        void Initialize(IEventStore eventStore, ISnapshotStore snapshotStore);
    }
}