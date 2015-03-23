using MS.EventSourcing.Infrastructure.Domain;
using MS.EventSourcing.Infrastructure.UnitTests.Events;

namespace MS.EventSourcing.Infrastructure.UnitTests.Aggregates
{
    public class Customer : AggregateRoot, IApplyDomainEvent<CustomerCreated>, IApplyDomainEvent<CustomerNameChanged>
    {
        public string Name { get; set; }

        internal void CreateCustomer(string name)
        {
            ApplyEvent(new CustomerCreated {Name = name});
        }

        internal void ChangeCustomerName(string name)
        {
            ApplyEvent(new CustomerNameChanged { Name = name });
        }

        void IApplyDomainEvent<CustomerCreated>.Apply(CustomerCreated domainEvent, bool isNew)
        {
            Name = domainEvent.Name;
        }

        void IApplyDomainEvent<CustomerNameChanged>.Apply(CustomerNameChanged domainEvent, bool isNew)
        {
            Name = domainEvent.Name;
        }
    }
}
