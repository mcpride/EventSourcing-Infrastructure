using System;
using System.Collections.Concurrent;
using MS.EventSourcing.Infrastructure.Domain;
using MS.Infrastructure;

namespace MS.EventSourcing.Infrastructure.EventHandling
{
    /// <summary>
    /// In memory snapshot store used for testing
    /// </summary>
    public class InMemorySnapshotStore : ISnapshotStore
    {
        private readonly static ConcurrentDictionary<Uuid, object> Snapshots = new ConcurrentDictionary<Uuid, object>();
 
        public Snapshot<T> GetSnapshot<T>(Uuid aggregateRootId)
        {
            return Snapshots.ContainsKey(aggregateRootId)
                ? (Snapshot<T>)Snapshots[aggregateRootId]
                : null;
        }

        public void SaveSnapshot<T>(Snapshot<T> snapshot)
        {
            if (snapshot == null) throw new ArgumentNullException("snapshot");
            Snapshots[snapshot.AggregateRootId] = snapshot;
        }

        public void Clear()
        {
            Snapshots.Clear();
        }
    }
}