using System;
using Newtonsoft.Json;
using MS.EventSourcing.Infrastructure.Domain;
using MS.EventSourcing.Infrastructure.EF.Models;
using MS.EventSourcing.Infrastructure.EventHandling;
using MS.Infrastructure;

namespace MS.EventSourcing.Infrastructure.EF
{
    public class SnapshotStore: ISnapshotStore
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

        protected readonly Func<IRepository> GetRepository = () => new Repository();
        private readonly IRepositoryFactory _repositoryFactory;

        public SnapshotStore()
        {
            GetRepository = () =>
                {
                    if (_repositoryFactory != null) return _repositoryFactory.NewRepository();
                    var repo = new Repository();
                    repo.Initialize();
                    return repo;
                };
        }

        public SnapshotStore(IRepositoryFactory repositoryFactory)
            : this()
        {
            _repositoryFactory = repositoryFactory;
        }


        /// <summary>
        /// Gets a snapsnot for a given Id if one exists
        /// </summary>
        /// <typeparam name="T">Type of snapshot detail</typeparam>
        /// <param name="aggregateRootId">ID of the aggregate the snapshot was taken from</param>
        /// <returns>Snapshot instance</returns>
        public Snapshot<T> GetSnapshot<T>(Uuid aggregateRootId)
        {
            var snapshotType = typeof (T).Name;

            var detail = GetRepository().GetSnapshot(aggregateRootId.AsGuid, snapshotType);

            if (detail == null)
            {
                return null;
            }

            return new Snapshot<T>
            {
                AggregateRootId = new Uuid {AsGuid = detail.AggregateRootId},
                Version = detail.Version,
                Data = JsonConvert.DeserializeObject<T>(detail.SnapshotData, SerializerSettings)
            };
        }

        /// <summary>
        /// Saves or updates the current snapshot for a given aggregate
        /// </summary>
        /// <typeparam name="T">Type of snapshot</typeparam>
        /// <param name="snapshot">Snapshot instance</param>
        public void SaveSnapshot<T>(Snapshot<T> snapshot)
        {
            var snapshotStream = new SnapshotStream
            {
                AggregateRootId = snapshot.AggregateRootId.AsGuid,
                Version = snapshot.Version,
                DateCreated = DateTime.UtcNow,
                SnapshotType = typeof(T).Name,
                SnapshotData = JsonConvert.SerializeObject(snapshot.Data, SerializerSettings)
            };

            GetRepository().InsertSnapshot(snapshotStream);
        }
    }
}
