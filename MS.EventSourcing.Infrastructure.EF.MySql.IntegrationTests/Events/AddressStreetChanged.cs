using MS.EventSourcing.Infrastructure.EventHandling;

namespace MS.EventSourcing.Infrastructure.EF.MySql.IntegrationTests.Events
{
    public class AddressStreetChanged: DomainEntityEvent
    {
        public string Street { get; set; }
    }
}
