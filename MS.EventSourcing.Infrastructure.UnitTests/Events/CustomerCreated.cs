using MS.EventSourcing.Infrastructure.EventHandling;

namespace MS.EventSourcing.Infrastructure.UnitTests.Events
{
    public class CustomerCreated : DomainEvent
    {
        public string Name { get; set; }
    }
}
