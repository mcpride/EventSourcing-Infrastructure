using System;
using MS.Infrastructure;

namespace MS.EventSourcing.Infrastructure.Domain
{
    /// <summary>
    /// Snapshots capture the state of an aggregate root at a moment in time.  When an 
    /// aggregate root is reloaded, it can be replayed from the snapshot forward
    /// insted of replaying the entire event stream.
    /// </summary>
    /// <typeparam name="T">Type of object containing snapshot data</typeparam>
    public class Snapshot<T> //where T : class
    {
        public Snapshot()
        {
        }

        public Snapshot(Uuid aggregateRoodId, long version, T data)
        {
            AggregateRootId = aggregateRoodId;
            Version = version;
            Data = data;
        }

        public Uuid AggregateRootId { get; set; }
        public long Version { get; set; }
        public T Data { get; set; }
    }
}