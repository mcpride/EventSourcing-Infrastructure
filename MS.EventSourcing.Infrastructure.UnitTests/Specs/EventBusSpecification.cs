using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MS.QualityTools.UnitTestFramework.Specifications;
using MS.EventSourcing.Infrastructure.EventHandling;
using MS.EventSourcing.Infrastructure.MassTransit;
using MS.EventSourcing.Infrastructure.UnitTests.Events;
using MS.EventSourcing.Infrastructure.UnitTests.Handlers;

namespace MS.EventSourcing.Infrastructure.UnitTests.Specs
{
    [TestClass]
    [SpecificationDescription("Event bus specification")]
    public class EventBusSpecification : Specification
    {
        [TestMethod]
        [ScenarioDescription("Create and initialize event bus with loopback")]
        public void Create_and_initialize_event_bus_with_loopback()
        {
            Given("an event bus", testContext => testContext.State.EventBus = new EventBus())
            .Then("the event bus should be initializable", testContext =>
            {
                var eventBus = (EventBus)testContext.State.EventBus;
                eventBus.Initialize(null, sbc => sbc.ReceiveFrom(new Uri("loopback://localhost/queue")));
            });
        }

        [TestMethod]
        [ScenarioDescription("Dispose created event bus")]
        public void Dispose_created_event_bus()
        {
            EventBus eventBus = null;
            Given("a created event bus", testContext => eventBus = new EventBus())
            .Then("the event bus should be disposable", testContext => eventBus.Dispose());
        }

        [TestMethod]
        [ScenarioDescription("Publishing 2 events over event bus with registered consumer will be handled")]
        public void Publishing_2_events_over_event_bus_with_registered_consumer_will_be_handled()
        {
            EventBus eventBus = null;

            Given("an event bus", testContext => eventBus = new EventBus())
            .And("the event bus is initialized with loopback",
                testContext =>
                {
                    EventBusTestConsumer.HandledEvents = 0;
                    eventBus.Initialize(ssbsc => ssbsc.Consumer(() => new EventBusTestConsumer()).Permanent(),
                        sbc => sbc.ReceiveFrom(new Uri("loopback://localhost/queue")));
                }
            )
            .And("the event bus has a valid consumer", testContext => { })
            .When("2 events will be published over event bus", testContext => eventBus.PublishEvents(
                new List<DomainEvent>
                {
                    new CustomerCreated(), 
                    new CustomerNameChanged()
                }))
            .Then("both events should be handled", testContext =>
            {
                for (var i = 0; i < 30; i++)
                {
                    if (EventBusTestConsumer.HandledEvents > 1) break;
                    Thread.Sleep(100);
                }
                Thread.Sleep(500);
                EventBusTestConsumer.HandledEvents.Should().Be(2);
            });
        }
    }
}
