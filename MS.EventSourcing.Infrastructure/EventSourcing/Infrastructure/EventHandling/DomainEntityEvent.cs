using MS.Infrastructure;
namespace MS.EventSourcing.Infrastructure.EventHandling
{
    /// <summary>
    /// Represents an event fired by entites contained within an aggregate
    /// </summary>
    public abstract class DomainEntityEvent : DomainEvent
    {
        public Uuid EntityId { get; set; }
    }
}