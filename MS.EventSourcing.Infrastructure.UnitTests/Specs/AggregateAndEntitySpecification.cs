using System.Collections.Generic;
using System.Linq;
using MS.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MS.QualityTools.UnitTestFramework.Specifications;
using MS.EventSourcing.Infrastructure.EventHandling;
using MS.EventSourcing.Infrastructure.UnitTests.Aggregates;
using MS.EventSourcing.Infrastructure.UnitTests.Entities;
using MS.EventSourcing.Infrastructure.UnitTests.Events;

namespace MS.EventSourcing.Infrastructure.UnitTests.Specs
{
    [TestClass]
    [SpecificationDescription("Aggregate and Entity specification")]
    public class AggregateAndEntitySpecification : Specification
    {
        [TestMethod]
        [ScenarioDescription("AggregateRoot ApplyEvent method test")]
        public void AggregateRootApplyEventTest()
        {
            Given("a Customer AggregateRoot", context =>
                {
                    context.State.AggregatRoot = new Customer();
                })
            .When("a CustomerCreated event was applied", context =>
                {
                    var domainEvent = new CustomerCreated {Name = "Hans Wurst"};
                    context.State.Event = domainEvent;
                    ((Customer) context.State.AggregatRoot).ApplyEvent(domainEvent);
                })
            .Then("the Customer AggregateRoot should have the Name property set to the same of the event",
                context => (((Customer) context.State.AggregatRoot).Name == "Hans Wurst"))
            .And("the event could be found in applied events list", context => 
                ((Customer)context.State.AggregatRoot).AppliedEvents.Contains(((CustomerCreated)context.State.Event)));
        }

        [TestMethod]
        [ScenarioDescription("AggregateRoot ReplayEvents test")]
        public void AggregateRootReplayEventsTest()
        {
            Given("a Customer AggregateRoot", context =>
                {
                    var customer = new Customer {Name = "Mister X"};
                    context.State.Customer = customer;
                    var address = new Address(customer, Uuid.NewId()) {Street = "Berliner Allee"};
                    context.State.Address = address;
                })
            .When("a list of 3 events was replayed", context =>
                ((Customer)context.State.Customer).ReplayEvents(new List<DomainEvent>
                {
                    new CustomerCreated { Name = "Hans Wurst" },
                    new AddressStreetChanged {Street = "Industriestraße", EntityId = context.State.Address.Id },
                    new CustomerNameChanged { Name = "Karl Obst" }
                }))
            .Then("the Customer AggregateRoot should have the Name property set to the same of the last domain event",context => 
                (((Customer)context.State.Customer).Name == "Karl Obst"));
        }
    }
}
