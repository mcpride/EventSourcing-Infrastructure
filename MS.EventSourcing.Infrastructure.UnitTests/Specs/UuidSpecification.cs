using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MS.QualityTools.UnitTestFramework.Specifications;
using MS.Infrastructure;

namespace MS.EventSourcing.Infrastructure.UnitTests.Specs
{
    [TestClass]
    [SpecificationDescription("Uuid specification")]
    public class UuidSpecification: Specification
    {
        [TestMethod]
        [ScenarioDescription("2 Uuids created from same Guid are equal")]  
        public void Two_Uuids_from_same_Guid_are_equal()
        {
            Given("a new Guid", context => context.State.guid = Guid.NewGuid())
            .And("a first Uuid initialized from this new Guid",
                context => context.State.uuid1 = new Uuid {AsGuid = context.State.guid})
            .And("a second Uuid initialized from this new Guid",
                context => context.State.uuid2 = new Uuid {AsGuid = context.State.guid})
            .When("both Uuids will be compared by method 'Equals'",
                context => context.State.result = (context.State.uuid1.Equals(context.State.uuid2)))
            .Then("the equation result should be true", context => context.State.result);
        }

        [TestMethod]
        [ScenarioDescription("2 Uuids created from same Guid are same")]
        public void Two_Uuids_from_same_Guid_are_same()
        {
            Given("a new Guid", context => context.State.guid = Guid.NewGuid())
            .And("a first Uuid initialized from this new Guid",
                context => context.State.uuid1 = new Uuid { AsGuid = context.State.guid })
            .And("a second Uuid initialized from this new Guid",
                context => context.State.uuid2 = new Uuid { AsGuid = context.State.guid })
            .When("both Uuids will be compared by operator '=='",
                context => context.State.result = (((Uuid)context.State.uuid1) == ((Uuid)context.State.uuid2)))
            .Then("the equation result should be true", context => context.State.result);
        }

        [TestMethod]
        [ScenarioDescription("2 different Uuids created from different Guids are not the same")]
        public void Two_different_Uuids_from_same_Guid_are_same()
        {
            Given("a first Uuid initialized with a new Guid",
                context => context.State.uuid1 = new Uuid { AsGuid = Guid.NewGuid() })
            .And("a second Uuid initialized with a new Guid",
                context => context.State.uuid2 = new Uuid { AsGuid = Guid.NewGuid() })
            .When("both Uuids will be compared by operator '!='",
                context => context.State.result = (((Uuid)context.State.uuid1) != ((Uuid)context.State.uuid2)))
            .Then("the equation result should be true", context => context.State.result);
        }

        [TestMethod]
        [ScenarioDescription("Two Uuids initialized by 'NewId' are different")]
        public void Two_Uuids_initialized_by_NewId_are_different()
        {
            Given("a first Uuid initialized with 'NewId'",
                context => context.State.uuid1 = Uuid.NewId())
            .And("a second Uuid initialized with 'NewId'",
                context => context.State.uuid2 = Uuid.NewId())
            .When("both Uuids will be compared",
                context => context.State.result = (((Uuid)context.State.uuid1) == ((Uuid)context.State.uuid2)))
            .Then("they should be different", context => !context.State.result);
        }

        [TestMethod]
        [ScenarioDescription("Two Uuids initialized by 'Empty' are equal")]
        public void Two_Uuids_initialized_by_Empty_are_equal()
        {
            Given("a first Uuid initialized with 'Empty'",
                context => context.State.uuid1 = Uuid.Empty())
            .And("a second Uuid initialized with 'Empty'",
                context => context.State.uuid2 = Uuid.Empty())
            .When("both Uuids will be compared",
                context => context.State.result = (((Uuid)context.State.uuid1) == ((Uuid)context.State.uuid2)))
            .Then("they should be equal", context => context.State.result);
        }
    }
}
