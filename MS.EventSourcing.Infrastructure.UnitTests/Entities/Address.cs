using System;
using MS.EventSourcing.Infrastructure.Domain;
using MS.EventSourcing.Infrastructure.UnitTests.Events;

namespace MS.EventSourcing.Infrastructure.UnitTests.Entities
{
    public class Address : Entity, IApplyDomainEntityEvent<AddressStreetChanged>
    {
        public string Street { get; set; }
        public string ZipCode { get; set; }

        public Address(AggregateRoot parent, Guid entityId)
            : base(parent, entityId)
        {
        }

        void IApplyDomainEntityEvent<AddressStreetChanged>.Apply(AddressStreetChanged domainEntityEvent, bool isNew)
        {
            Street = domainEntityEvent.Street;
        }
    }
}
