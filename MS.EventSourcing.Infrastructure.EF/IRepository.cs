using System;
using System.Collections.Generic;
using MS.EventSourcing.Infrastructure.EF.Models;

namespace MS.EventSourcing.Infrastructure.EF
{
    /// <summary>
    /// Represents event stream and snapshot persistance
    /// </summary>
    public interface IRepository : IDisposable
    {
        void Initialize(string contextInfo);
        IEnumerable<EventStream> GetEventStream(Guid aggregateRoodId, string aggregateType, long startVersion);
        void InsertEvents(EventStream eventStream);
        SnapshotStream GetSnapshot(Guid aggregateRootId, string snapshotType);
        void InsertSnapshot(SnapshotStream snapshot);
    }
}