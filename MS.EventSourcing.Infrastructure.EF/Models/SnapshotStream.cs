using System;

namespace MS.EventSourcing.Infrastructure.EF.Models
{
    public class SnapshotStream
    {
        public long Id { get; set; }
        public Guid AggregateRootId { get; set; }
        public DateTime DateCreated { get; set; }
        public long Version { get; set; }
        public string SnapshotType { get; set; }
        public string SnapshotData { get; set; }
    }
}
