using System;
using System.Collections.Generic;
using System.Linq;
using ReflectionMagic;
using MS.EventSourcing.Infrastructure.EventHandling;
using MS.Infrastructure;

namespace MS.EventSourcing.Infrastructure.Domain
{
    /// <summary>
    /// Repository for loading aggregate root objects from a stream
    /// of events and snapshots
    /// </summary>
    public class DomainRepository : IDomainRepository
    {
        private ISnapshotStore _snapshotStore;
        private IEventStore _eventStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainRepository"/> class.
        /// </summary>
        public DomainRepository()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainRepository"/> class 
        /// and initializes the repository with snapshot and event storage.
        /// </summary>
        /// <param name="eventStore">Event storage object</param>
        /// <param name="snapshotStore">Snapshot storage object</param>
        public DomainRepository(IEventStore eventStore, ISnapshotStore snapshotStore)
        {
            Initialize(eventStore, snapshotStore);
        }

        private void EnsureInitialized()
        {
            if ((_eventStore == null) || (_snapshotStore == null))
            {
                throw new InvalidOperationException(StringResources.ErrDomainRepositoryNotInitialized());
            }
        }

        /// <summary>
        /// Initializes the repository with snapshot and event storage.
        /// </summary>
        /// <param name="eventStore">Event storage object</param>
        /// <param name="snapshotStore">Snapshot storage object</param>
        public void Initialize(IEventStore eventStore, ISnapshotStore snapshotStore)
        {
            _snapshotStore = snapshotStore;
            _eventStore = eventStore;
        }

        /// <summary>
        /// Gets an aggregate root instance of type T for a given ID
        /// </summary>
        /// <typeparam name="T">Type of class to create and hydrate</typeparam>
        /// <param name="aggregateRootId">ID of the persisted aggregate root instance</param>
        /// <returns></returns>
        public T GetById<T>(Uuid aggregateRootId) where T : AggregateRoot, new()
        {
            EnsureInitialized();
            
            var aggregateRoot = new T {Id = aggregateRootId};

            // Attempts to load the most recent snapshot for the given ID
            var snapshot = GetSnapshot(aggregateRoot, aggregateRootId);

            if (snapshot != null)
            {
                // Call the LoadFromSnapshot method dynamically with the generic snapshot instance
                GetType().AsDynamicType().LoadFromSnapshot(aggregateRoot, snapshot);
            }

            var aggregateRootType = typeof(T).Name;

            // Gets the event stream for the given ID.  Events are loaded based on the
            // root's current version which allows for replaying only a subset of events
            // after a snapshot was taken.
            var events = _eventStore.GetEvents(aggregateRootId, aggregateRootType, aggregateRoot.Version);

            var currentEvents = UpdateEventVersions(events);

            // Replays all events to bring the root up to current version
            aggregateRoot.ReplayEvents(currentEvents);

            return aggregateRoot.Version == 0 
                ? null
                : aggregateRoot;
        }

        /// <summary>
        /// Loads a snapshot from the snapshot store and initializes the aggregate
        /// root's history from the snapshot's data bringing the aggregate root up
        /// to the current version of the snapshot.
        /// </summary>
        /// <param name="aggregateRoot">Snapshot-capable aggregate root.</param>
        /// <param name="snapshot">Snapshot of generic type T to dynamically load</param>
        public static void LoadFromSnapshot(AggregateRoot aggregateRoot, object snapshot)
        {
            if (!(aggregateRoot is IAggregateRootWithSnapshot))
            {
                return;
            }

            var dynamicRoot = aggregateRoot.AsDynamic();
            
            // Initialize the root which will set the current version
            dynamicRoot.InitializeFromSnapshot(snapshot);

            // Bring the root's state up to the snapshot's state and version
            dynamicRoot.LoadFromSnapshot(snapshot.AsDynamic().Data);
        }

        /// <summary>
        /// Calls the generic GetSnapshot method of the snapshot store object.
        /// </summary>
        /// <param name="root">Snapshot-capable aggregate Root to load</param>
        /// <param name="aggregateRootId">ID of the persisted aggregate root instance</param>
        /// <returns>Snapshot of Type T casted as an object</returns>
        public object GetSnapshot(AggregateRoot root, Uuid aggregateRootId)
        {
            EnsureInitialized();
            
            if (!(root is IAggregateRootWithSnapshot))
            {
                return null;
            }

            var casted = (IAggregateRootWithSnapshot)root;

            var snapshotType = casted.GetGenericType();
            var method = _snapshotStore.GetType().GetMethod("GetSnapshot").MakeGenericMethod(snapshotType);

            return method.Invoke(_snapshotStore, new object[] { aggregateRootId });
        }

        /// <summary>
        /// Upgrades outdated events to their current counterpart.
        /// </summary>
        /// <param name="events"></param>
        public IEnumerable<DomainEvent> UpdateEventVersions(IEnumerable<DomainEvent> events)
        {
            if (events == null)
            {
                return null;
            }

            var currentEvents = events.ToList();

            for (var i = 0; i < currentEvents.Count(); i++)
            {
                // Continue upgrading until the current version is found.
                DomainEvent nextVersion;
                while ((nextVersion = currentEvents[i].UpgradeVersion()) != null)
                {
                    currentEvents[i] = nextVersion;
                }
            }

            return currentEvents;
        }

        public static class StringResources
        {
            public static Func<string> ErrDomainRepositoryNotInitialized = () => "Domain repository has not been initialized properly!";
        }
    }
}