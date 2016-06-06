using System;
using System.Linq;
using ReflectionMagic;
using MS.EventSourcing.Infrastructure.EventHandling;
using MS.Infrastructure;

namespace MS.EventSourcing.Infrastructure.Domain
{
    public class DomainContext : IDomainContext
    {
        private readonly IDomainRepository _repository;
        private readonly IEventStore _eventStore;
        private readonly IEventBus _eventBus;
        private readonly ISnapshotStore _snapshotStore;
 
        /// <summary>
        /// Initializes a domain context
        /// </summary>
        public DomainContext(IEventBus eventBus, IEventStore eventStore, ISnapshotStore snapshotStore, IDomainRepository domainRepository)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
            _snapshotStore = snapshotStore;
            _repository = domainRepository;
        }

        /// <summary>
        /// Get's a persisted aggregate root by a given id
        /// </summary>
        /// <typeparam name="T">Type of aggregate root to load</typeparam>
        /// <param name="id">Id of the root object</param>
        /// <returns>Aggregate root of type T with events and snapshots replayed</returns>
        public virtual T GetById<T>(Uuid id) where T : AggregateRoot, new()
        {
            return _repository.GetById<T>(id);
        }

        /// <summary>
        /// Persists event stream, publishes events, and creates snapshots
        /// </summary>
        /// <param name="root">Aggregate root to persist</param>
        /// <param name="broadcastOnly">Do not persist the evnet; only publish to event handlers.</param>
        public virtual void Finalize(AggregateRoot root, bool broadcastOnly = false)
        {
            if (root == null) throw new ArgumentNullException("root");
            if (!broadcastOnly)
            {
                var eventsToStore = root.AppliedEvents.Where(domainEvent => !(domainEvent is IExternalEvent));
                // Persist events to the event store
                _eventStore.Insert(root.Id, root.GetType().Name, eventsToStore);
            }

            // Publish events to interested parties
            _eventBus.PublishEvents(root.AppliedEvents);

            if (!broadcastOnly)
            {
                TryCreateSnapshot(root);
            }
        }

        /// <summary>
        /// Optionally creates a snapshot of the aggregate root
        /// </summary>
        /// <param name="aggregateRoot">Aggregate root instance to snapshot</param>
        protected bool TryCreateSnapshot(AggregateRoot aggregateRoot)
        {
            if ( !(aggregateRoot is IAggregateRootWithSnapshot))
            {
                return false;
            }

            var dynamicRoot = aggregateRoot.AsDynamic();

            // Only create a snapshot of the root requires it
            if (!dynamicRoot.ShouldCreateSnapshot())
            {
                return false;
            }

            var snapshot = ((IAggregateRootWithSnapshot) aggregateRoot).CreateGenericSnapshot();

            if (snapshot == null)
            {
                return false;
            }

            // Dynamically invoke the SaveSnapshot<T> method of the snapshot store
            _snapshotStore.InvokeGenericMethod("SaveSnapshot", ((IAggregateRootWithSnapshot) aggregateRoot).GetGenericType(), snapshot);
            return true;
        }
    }
}
