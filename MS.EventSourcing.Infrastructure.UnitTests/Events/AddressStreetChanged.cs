using MS.EventSourcing.Infrastructure.EventHandling;

namespace MS.EventSourcing.Infrastructure.UnitTests.Events
{
    public class AddressStreetChanged: DomainEntityEvent
    {
        public string Street { get; set; }
    }
}
