using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Magnum.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MS.QualityTools.UnitTestFramework.Specifications;
using MS.EventSourcing.Infrastructure.Domain;
using MS.EventSourcing.Infrastructure.EventHandling;
using MS.EventSourcing.Infrastructure.UnitTests.Aggregates;
using MS.EventSourcing.Infrastructure.UnitTests.Events;
using MS.Infrastructure;

namespace MS.EventSourcing.Infrastructure.UnitTests.Specs
{
    [TestClass]
    [SpecificationDescription("Domain context specification")]
    public class DomainContextSpecification : Specification
    {
        [TestMethod]
        [ScenarioDescription("Finalization without aggregate throws error")]
        public void Finalization_without_aggregate_throws_error()
        {
            Given("an initialized domain context", 
                testContext =>
                {
                    var eventBus = new Mock<IEventBus>().Object;
                    var eventStore = new Mock<IEventStore>().Object;
                    var snapshotStore = new Mock<ISnapshotStore>().Object;
                    var domainRepository = new Mock<IDomainRepository>().Object;
                    var domainContext = new DomainContext(eventBus, eventStore, snapshotStore, domainRepository);
                    testContext.State.DomainContext = domainContext;
                })
            .When("Finalize without an aggregate is called", 
                testContext =>
                {
                    Action whenAction = () => ((DomainContext) testContext.State.DomainContext).Finalize(null);
                    testContext.State.When = whenAction;
                })
            .Then("an ArgumentNullException should be thrown", 
                testContext => 
                    ((Action) testContext.State.When).ShouldThrow<ArgumentNullException>());
        }

        [TestMethod]
        [ScenarioDescription("GetById returns the right aggregate with the defined ID and type")]
        public void GetById_returns_the_right_aggregate_with_the_defined_Id_and_type()
        {
            Given("a set of events with different aggregate ids in an event store",
                testContext =>
                {
                    var aggregateId = Uuid.NewId();
                    testContext.State.AggregateId = aggregateId;
                    var eventStore = new InMemoryEventStore();
                    eventStore.Insert(aggregateId, "Customer", new List<DomainEvent> {new CustomerCreated(), new CustomerNameChanged()});
                    eventStore.Insert(aggregateId, "Customer", new List<DomainEvent> { new AddressStreetChanged() });
                    eventStore.Insert(Uuid.NewId(), "Customer", new List<DomainEvent> { new CustomerCreated() });
                    eventStore.Insert(Uuid.NewId(), "Customer", new List<DomainEvent> { new CustomerCreated() });
                    testContext.State.EventStore = eventStore;
                })
            .And("an empty snapshot store",
                testContext =>
                {
                    testContext.State.SnapshotStore = new InMemorySnapshotStore();
                })
            .And("a configured domain context",
                testContext =>
                {
                    var eventBus = new Mock<IEventBus>().Object;
                    IEventStore eventStore = testContext.State.EventStore;
                    ISnapshotStore snapshotStore = testContext.State.SnapshotStore;
                    var domainRepository = new DomainRepository(eventStore, snapshotStore);
                    var domainContext = new DomainContext(eventBus, eventStore, snapshotStore, domainRepository);
                    testContext.State.DomainContext = domainContext;
                })
            .When("GetById for a defined aggregate type with and events contained in the event store with the defined aggregate id is called", 
                testContext =>
                {
                    var aggregate =
                        ((DomainContext) testContext.State.DomainContext).GetById<Customer>(
                            testContext.State.AggregateId);
                    testContext.State.Aggregate = aggregate;
                })
            .Then("it should return a valid aggregate",
                testContext =>
                {
                    object obj = testContext.State.Aggregate;

                    obj.Should().NotBeNull();
                    obj.Should().BeOfType<Customer>();
                });
        }

        [TestMethod]
        [ScenarioDescription("GetById returns no aggregate with a different ID than defined")]
        public void GetById_returns_no_aggregate_with_a_different_ID_than_defined()
        {
            Given("a set of events with different aggregate ids in an event store",
                testContext =>
                {
                    var eventStore = new InMemoryEventStore();
                    eventStore.Insert(Uuid.NewId(), "Customer", new List<DomainEvent> { new CustomerCreated(), new CustomerNameChanged() });
                    eventStore.Insert(Uuid.NewId(), "Customer", new List<DomainEvent> { new CustomerCreated() });
                    eventStore.Insert(Uuid.NewId(), "Customer", new List<DomainEvent> { new CustomerCreated() });
                    testContext.State.EventStore = eventStore;
                })
            .And("an empty snapshot store",
                testContext =>
                {
                    testContext.State.SnapshotStore = new InMemorySnapshotStore();
                })
            .And("a configured domain context",
                testContext =>
                {
                    var eventBus = new Mock<IEventBus>().Object;
                    IEventStore eventStore = testContext.State.EventStore;
                    ISnapshotStore snapshotStore = testContext.State.SnapshotStore;
                    var domainRepository = new DomainRepository(eventStore, snapshotStore);
                    var domainContext = new DomainContext(eventBus, eventStore, snapshotStore, domainRepository);
                    testContext.State.DomainContext = domainContext;
                })
            .When("GetById for an aggregate with unknown id is called",
                testContext =>
                {
                    var aggregate =
                        ((DomainContext)testContext.State.DomainContext).GetById<Customer>(Uuid.NewId());
                    testContext.State.Aggregate = aggregate;
                })
            .Then("it should return null",
                testContext =>
                {
                    object obj = testContext.State.Aggregate;
                    obj.Should().BeNull();
                });
        }

        
        [TestMethod]
        [ScenarioDescription("Finalize without broadcast only option saves the applied events into event store")]
        public void Finalize_without_broadcast_only_option_saves_the_applied_events_into_event_store()
        {
            Given("an empty eventstore",
                testContext =>
                {
                    testContext.State.EventStore = new InMemoryEventStore();
                })
            .And("an empty snapshot store",
                testContext =>
                {
                    testContext.State.SnapshotStore = new InMemorySnapshotStore();
                })
            .And("a customer aggregate with the 2 new events 'CustomerCreated' and 'CustomerNameChanged' in AppliedEvents",
                testContext =>
                {
                    var customer = new Customer();
                    customer.CreateCustomer("hans");
                    customer.ChangeCustomerName("horst");
                    testContext.State.Aggregate = customer;
                })
            .And("a configured domain context",
                testContext =>
                {
                    var eventBus = new Mock<IEventBus>().Object;
                    IEventStore eventStore = testContext.State.EventStore;
                    ISnapshotStore snapshotStore = testContext.State.SnapshotStore;
                    var domainRepository = new DomainRepository(eventStore, snapshotStore);
                    var domainContext = new DomainContext(eventBus, eventStore, snapshotStore, domainRepository);
                    testContext.State.DomainContext = domainContext;
                })
            .When("Finalize without broadcast only option is called for the customer aggregate",
                testContext => ((DomainContext)testContext.State.DomainContext).Finalize(testContext.State.Aggregate))
            .Then("the event store should contain these 2 events 'CustomerCreated' and 'CustomerNameChanged'",
                testContext =>
                {
                    var eventStore = (InMemoryEventStore)testContext.State.EventStore;
                    var events = eventStore.GetEvents(((Customer)testContext.State.Aggregate).Id, typeof(Customer).Name, 0);
                    events.Should().NotBeEmpty();
                    events.First().Should().BeOfType<CustomerCreated>();
                    events.Last().Should().BeOfType<CustomerNameChanged>();
                    events.First().CastAs<CustomerCreated>().Name.Should().Be("hans");
                    events.Last().CastAs<CustomerNameChanged>().Name.Should().Be("horst");
                });
        }
    }
}
