using System;

namespace MS.EventSourcing.Infrastructure.EF.Models
{
    public class EventStream
    {
        public long Id { get; set; }
        public Guid AggregateRootId { get; set; }
        public DateTime DateCreated { get; set; }
        public long SequenceStart { get; set; }
        public long SequenceEnd { get; set; }
        public string AggregateType { get; set; }
        public string EventData { get; set; }
    }
}
