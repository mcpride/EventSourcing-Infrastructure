using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using MS.EventSourcing.Infrastructure.EF.Models;
using MS.EventSourcing.Infrastructure.EventHandling;
using MS.Infrastructure;

namespace MS.EventSourcing.Infrastructure.EF
{
    /// <summary>
    /// MySql event storage provider
    /// </summary>
    public class EventStore : IEventStore
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

        protected readonly Func<IRepository> GetRepository;
        private readonly IRepositoryFactory _repositoryFactory;

        public EventStore()
        {
            GetRepository = () =>
                {
                    if (_repositoryFactory != null) return _repositoryFactory.NewRepository();
                    var repo = new Repository();
                    repo.Initialize();
                    return repo;
                };
        }

        public EventStore(IRepositoryFactory repositoryFactory)
            : this()
        {
            _repositoryFactory = repositoryFactory;
        }

        /// <summary>
        /// Queries MySql for a stream of events for a given ID.
        /// </summary>
        /// <param name="aggregateRootId">ID of the aggregate root instance</param>
        /// <param name="aggregateType">Aggregate type</param>
        /// <param name="startVersion">Starting event version of the event sequence range.</param>
        /// <returns>Ordered list of domain events</returns>
        public IEnumerable<DomainEvent> GetEvents(Uuid aggregateRootId, string aggregateType, long startVersion)
        {
            var events = new List<DomainEvent>();

            var repository = GetRepository();
            var streams = repository.GetEventStream(aggregateRootId.AsGuid, aggregateType, startVersion);

            foreach (var streamEvents in streams.OrderBy(evt => evt.SequenceStart)
                .Select(stream => JsonConvert.DeserializeObject<IEnumerable<DomainEvent>>(stream.EventData, SerializerSettings)))
            {
                events.AddRange(streamEvents);
            }

            return events;
        }

        /// <summary>
        /// Inserts a new list of events into MySql
        /// </summary>
        /// <param name="aggregateRootId">Aggregate root ID</param>
        /// <param name="aggregateType">Aggregate type</param>
        /// <param name="domainEvents">List of events to insert</param>
        public void Insert(Uuid aggregateRootId, string aggregateType, IEnumerable<DomainEvent> domainEvents)
        {
            var events = domainEvents.ToList();

            var firstEvent = events.First();
            var lastEvent = events.Last();

            var stream = new EventStream
            {
                AggregateRootId = aggregateRootId.AsGuid,
                DateCreated = DateTime.UtcNow,
                SequenceStart = firstEvent.Sequence,
                SequenceEnd = lastEvent.Sequence,
                AggregateType = aggregateType,
                EventData = JsonConvert.SerializeObject((dynamic)events, SerializerSettings)
            };
            var repository = GetRepository();
            repository.InsertEvents(stream);
        }
    }
}
