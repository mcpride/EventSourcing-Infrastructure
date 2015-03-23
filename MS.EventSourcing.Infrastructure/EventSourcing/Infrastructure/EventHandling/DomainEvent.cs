using System;
using System.ComponentModel;

namespace MS.EventSourcing.Infrastructure.EventHandling
{
    /// <summary>
    /// Represents an event fired by an aggregate root domain object
    /// </summary>
    public abstract class DomainEvent
    {
        [DefaultValue(0)]
        public long Sequence { get; set; }

        public DateTime EventDate { get; set; }

        public virtual DomainEvent UpgradeVersion()
        {
            return null;
        }
    }
}
