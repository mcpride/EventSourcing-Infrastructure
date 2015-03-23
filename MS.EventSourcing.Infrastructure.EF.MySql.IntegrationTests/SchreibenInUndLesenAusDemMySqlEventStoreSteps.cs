using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MS.EventSourcing.Infrastructure.EF.MySql.IntegrationTests.Events;
using MS.EventSourcing.Infrastructure.EventHandling;
using MS.Infrastructure;
using TechTalk.SpecFlow;

namespace MS.EventSourcing.Infrastructure.EF.MySql.IntegrationTests
{
    [Binding]
    public class SchreibenInUndLesenAusDemMySqlEventStoreSteps
    {
        [Given(@"ein MySqlEventStore")]
        public void AngenommenEinMySqlEventStore()
        {
            ScenarioContext.Current.Set<IEventStore>(new EventStore());
        }

        [Given(@"eine definierte Aggregat-ID")]
        public void AngenommenEineDefinierteAggregat_ID()
        {
            ScenarioContext.Current.Set(Uuid.NewId());
        }

        [Given(@"eine Liste von Domänen-Ereignissen zu dieser Aggregat-ID")]
        public void AngenommenEineListeVonDomanen_EreignissenZuDieserAggregat_ID()
        {
            var events = new List<DomainEvent>
            {
                new CustomerCreated {EventDate = DateTime.UtcNow, Name = "Hans Wurst", Sequence = 0},
                new CustomerNameChanged {EventDate = DateTime.UtcNow, Name = "Hans Würstchen", Sequence = 0}
            };
            ScenarioContext.Current.Set<IEnumerable<DomainEvent>>(events);
        }

        [When(@"ich diese Liste von Domänen-Ereignissen in den MySqlEventStore speichere")]
        public void WennIchDieseListeVonDomanen_EreignissenInDenMySqlEventStoreSpeichere()
        {
            var eventStore = ScenarioContext.Current.Get<IEventStore>();
            var aggregateId = ScenarioContext.Current.Get<Uuid>();
            var events = ScenarioContext.Current.Get<IEnumerable<DomainEvent>>();
            eventStore.Insert(aggregateId, "Customer", events);
        }

        [Then(@"sollte ich diese Domänen-Ereignisse auch wieder auslesen können")]
        public void DannSollteIchDieseDomanen_EreignisseAuchWiederAuslesenKonnen()
        {
            var eventStore = ScenarioContext.Current.Get<IEventStore>();
            var aggregateId = ScenarioContext.Current.Get<Uuid>();
            var expectedEvents = ScenarioContext.Current.Get<IEnumerable<DomainEvent>>().ToList();
            var actualEvents = eventStore.GetEvents(aggregateId, "Customer", 0).ToList();
            Assert.IsTrue(actualEvents.Count() == expectedEvents.Count());
            var i = 0;
            foreach (var actualEvent in actualEvents)
            {
                if (actualEvent.GetType() == typeof (CustomerCreated))
                {
                    Assert.AreEqual(((CustomerCreated)actualEvent).Name, ((CustomerCreated)expectedEvents[i]).Name);
                }
                if (actualEvent.GetType() == typeof(CustomerNameChanged))
                {
                    Assert.AreEqual(((CustomerNameChanged)actualEvent).Name, ((CustomerNameChanged)expectedEvents[i]).Name);
                }
                i++;
            }

        }
    }
}
