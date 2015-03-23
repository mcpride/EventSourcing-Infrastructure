using System;
using MS.Infrastructure;

namespace MS.EventSourcing.Infrastructure.Domain
{
    public interface IDomainContext
    {
        /// <summary>
        /// Get's a persisted aggregate root by a given id
        /// </summary>
        /// <typeparam name="T">Type of aggregate root to load</typeparam>
        /// <param name="id">Id of the root object</param>
        /// <returns>Aggregate root of type T with events and snapshots replayed</returns>
        T GetById<T>(Uuid id) where T : AggregateRoot, new();

        /// <summary>
        /// Persists event stream, publishes events, and creates snapshots
        /// </summary>
        /// <param name="root">Aggregate root to persist</param>
        /// <param name="broadcastOnly">Do not persist the evnet; only publish to event handlers.</param>
        void Finalize(AggregateRoot root, bool broadcastOnly = false);
    }
}