using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MS.EventSourcing.Infrastructure.EventHandling;
using MS.Infrastructure;

namespace MS.EventSourcing.Infrastructure.Domain
{
    /// <summary>
    /// Base class for creating domain objects
    /// </summary>
    public abstract class AggregateRoot
    {
        private readonly List<DomainEvent> _appliedEvents = new List<DomainEvent>();
        private readonly List<Entity> _entities = new List<Entity>();
        private static readonly IDictionary<string, MethodInfo> CachedLocalMethods = new ConcurrentDictionary<string, MethodInfo>();
        private static readonly IDictionary<string, MethodInfo> CachedEntityMethods = new ConcurrentDictionary<string, MethodInfo>(); 

        /// <summary>
        /// Creates a new aggregate root with a new ID
        /// </summary>
        protected AggregateRoot()
            : this(Uuid.NewId())
        { }

        /// <summary>
        /// Creates a new aggregate root
        /// </summary>
        /// <param name="id">Id of the root</param>
        protected AggregateRoot(Uuid id)
        {
            Id = id;
        }
        
        /// <summary>
        /// Instance's identifier
        /// </summary>
        public Uuid Id { get; protected internal set; }

        /// <summary>
        /// Current version of the instance
        /// </summary>
        public long Version { get; protected internal set; }

        /// <summary>
        /// Events that have been applied and await persistance
        /// </summary>
        public IEnumerable<DomainEvent> AppliedEvents { get { return _appliedEvents; } } 

        /// <summary>
        /// Applies an event to the instance augmenting the current version
        /// for each event applied
        /// </summary>
        /// <param name="domainEvent">Event to apply</param>
        /// <param name="isNew">True if the event is new to the event stream; otherwise false</param>
        public void ApplyEvent(DomainEvent domainEvent, bool isNew = true)
        {
            if (domainEvent == null) throw new ArgumentNullException("domainEvent");
            Version++;

            if (isNew)
            {
                domainEvent.Sequence = Version;
                domainEvent.EventDate = DateTime.UtcNow;
            }

            if (!(domainEvent is IExternalEvent))
            {
                // Call the apply method on the domain model instance
                ApplyEventToSelf(domainEvent, isNew);

                if (domainEvent is DomainEntityEvent)
                {
                    ApplyEventToEntities(domainEvent as DomainEntityEvent, isNew);
                }
            }

            // Save the event for persistance if it's new
            if (isNew)
            {
                _appliedEvents.Add(domainEvent);
            }
        }

        /// <summary>
        /// Replays a stream of events to bring the instance up to the current
        /// state and version
        /// </summary>
        /// <param name="events">Stream of events to replay</param>
        public void ReplayEvents(IEnumerable<DomainEvent> events)
        {
            if(events == null)
            {
                return;
            }

            foreach (var domainEvent in events)
            {
                ApplyEvent(domainEvent, false);
            }
        }

        /// <summary>
        /// Associates an entity with the aggregate root
        /// </summary>
        /// <param name="entity">Entity to associate</param>
        internal void Associate(Entity entity)
        {
            if (!_entities.Contains(entity))
            {
                _entities.Add(entity);
            }
        }

        /// <summary>
        /// Applies a domain event to the current instance
        /// </summary>
        /// <param name="domainEvent">Event to apply</param>
        /// <param name="isNew">True if the event is new to the event stream; otherwise false</param>
        private void ApplyEventToSelf(DomainEvent domainEvent, bool isNew)
        {
            ApplyMethodWithCaching(this, domainEvent, isNew, CachedLocalMethods, false);
        }

        /// <summary>
        /// Applies an entity event to the appropriate entity
        /// </summary>
        /// <param name="entityEvent">Entity event to apply</param>
        /// <param name="isNew">True if the event is new to the event stream; otherwise false</param>
        private void ApplyEventToEntities(DomainEntityEvent entityEvent, bool isNew)
        {
            var entity = _entities.FirstOrDefault(e => e.Id == entityEvent.EntityId);

            if (entity == null)
            {
                return;
            }

            ApplyMethodWithCaching(entity, entityEvent, isNew, CachedEntityMethods, true);
        }

        private void ApplyMethodWithCaching(object instanceToApply, DomainEvent eventToApply, bool isNew, IDictionary<string, MethodInfo> cache, bool toEntity)
        {
            var eventType = eventToApply.GetType();

            var localKey = string.Format("{0},{1}", GetType().FullName, eventType);
            MethodInfo method;

            // Check of the handler (method info) for this event has been cached
            if (!cache.TryGetValue(localKey, out method))
            {
                // Get the convention-based handler
                if (!toEntity)
                {
                    var applyDomainEventType = typeof(IApplyDomainEvent<>).MakeGenericType(eventType);
                    if (applyDomainEventType.IsInstanceOfType(instanceToApply))
                    {
                        method = applyDomainEventType.GetMethod("Apply");
                        cache.Add(localKey, method);
                    }
                }
                else if (eventToApply.GetType().IsSubclassOf(typeof(DomainEntityEvent)))
                {
                    var applyDomainEntityEventType = typeof(IApplyDomainEntityEvent<>).MakeGenericType(eventType);
                    if (applyDomainEntityEventType.IsInstanceOfType(instanceToApply))
                    {
                        method = applyDomainEntityEventType.GetMethod("Apply");
                        cache.Add(localKey, method);
                    }
                }
            }

            if (method != null)
            {
                method.Invoke(instanceToApply, new object[] { eventToApply, isNew });
            }
        }
    }
}