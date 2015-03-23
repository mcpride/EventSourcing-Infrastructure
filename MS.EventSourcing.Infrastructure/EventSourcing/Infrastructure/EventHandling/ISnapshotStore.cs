using MS.EventSourcing.Infrastructure.Domain;
using MS.Infrastructure;

namespace MS.EventSourcing.Infrastructure.EventHandling
{
    /// <summary>
    /// Represents a snapshot store for storing aggregate root snapshots
    /// </summary>
    public interface ISnapshotStore
    {
        Snapshot<T> GetSnapshot<T>(Uuid aggregateRootId);
        void SaveSnapshot<T>(Snapshot<T> snapshot);
    }
}
