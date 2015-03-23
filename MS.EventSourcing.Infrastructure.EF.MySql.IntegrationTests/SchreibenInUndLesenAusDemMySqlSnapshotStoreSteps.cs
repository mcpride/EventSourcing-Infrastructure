using Microsoft.VisualStudio.TestTools.UnitTesting;
using MS.EventSourcing.Infrastructure.Domain;
using MS.EventSourcing.Infrastructure.EF.MySql.IntegrationTests.Snaphots;
using MS.EventSourcing.Infrastructure.EventHandling;
using MS.Infrastructure;
using TechTalk.SpecFlow;

namespace MS.EventSourcing.Infrastructure.EF.MySql.IntegrationTests
{
    [Binding]
    public class SchreibenInUndLesenAusDemMySqlSnapshotStoreSteps
    {
        [Given(@"ein MySqlSnapshotStore")]
        public void AngenommenEinMySqlSnapshotStore()
        {
            ScenarioContext.Current.Set<ISnapshotStore>(new SnapshotStore());
        }

        [Given(@"eine Snapshotentität")]
        public void AngenommenEineSnapshotentitat()
        {
            //var customer = new Customer { Address = new CustomerAddress { Street = "Industriestraße"}, Name = "Hans Meiser"};
            var customer = new Customer {Name = "Hans Meiser", Address = {Street = "Industriestraße"}};
            ScenarioContext.Current.Set(customer);
        }

        [When(@"ich die Snapshotentität in den MySqlSnapshotStore speichere")]
        public void WennIchDieSnapshotentitatInDenMySqlSnapshotStoreSpeichere()
        {
            var id = Uuid.NewId();
            ScenarioContext.Current.Set(id);
            var store = ScenarioContext.Current.Get<ISnapshotStore>();
            var customer = ScenarioContext.Current.Get<Customer>();
            store.SaveSnapshot(new Snapshot<Customer>(id, 0, customer));
        }

        [Then(@"sollte ich diese Snapshotentität auch wieder auslesen können")]
        public void DannSollteIchDieseSnapshotentitatAuchWiederAuslesenKonnen()
        {
            var id = ScenarioContext.Current.Get<Uuid>();
            var store = ScenarioContext.Current.Get<ISnapshotStore>();
            var customer = ScenarioContext.Current.Get<Customer>();
            var snapshot = store.GetSnapshot<Customer>(id);
            Assert.IsNotNull(snapshot);
            Assert.IsNotNull(snapshot.Data);
            Assert.IsNotNull(snapshot.Data.Address);
            Assert.AreEqual(customer.Name, snapshot.Data.Name);
            Assert.AreEqual(customer.Address.Street, snapshot.Data.Address.Street);
        }
    }
}
