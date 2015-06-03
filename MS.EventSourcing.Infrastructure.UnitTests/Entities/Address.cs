using System;
using MS.EventSourcing.Infrastructure.Domain;
using MS.EventSourcing.Infrastructure.UnitTests.Events;
using MS.Infrastructure;

namespace MS.EventSourcing.Infrastructure.UnitTests.Entities
{
    public class Address : Entity, IApplyDomainEntityEvent<AddressStreetChanged>
    {
        public string Street { get; set; }
        public string ZipCode { get; set; }

        public Address(AggregateRoot parent, Uuid entityId)
            : base(parent, entityId)
        {
        }

        void IApplyDomainEntityEvent<AddressStreetChanged>.Apply(AddressStreetChanged domainEntityEvent, bool isNew)
        {
            Street = domainEntityEvent.Street;
        }
    }
}
