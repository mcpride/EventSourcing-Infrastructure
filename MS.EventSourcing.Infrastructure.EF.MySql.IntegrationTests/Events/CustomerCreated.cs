using MS.EventSourcing.Infrastructure.EventHandling;

namespace MS.EventSourcing.Infrastructure.EF.MySql.IntegrationTests.Events
{
    public class CustomerCreated : DomainEvent
    {
        public string Name { get; set; }
    }
}
