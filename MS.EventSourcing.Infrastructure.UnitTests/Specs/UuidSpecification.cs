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

        [TestMethod]
        [ScenarioDescription("Uuid created from Guid should return the same Guid in Uuid.AsGuid")]
        public void Uuid_created_from_Guid_should_return_the_same_Guid_in_AsGuid()
        {
            Given("a new Guid",
                context => context.State.guid = Guid.NewGuid())
            .And("a Uuid constructed from this Guid",
                context => context.State.uuid = new Uuid((Guid)(context.State.guid)))
            .When("both will be compared as Guid",
                context => context.State.result = (((Uuid)context.State.uuid).AsGuid == ((Guid)context.State.guid)))
            .Then("they should be equal", context => context.State.result);
        }

        [TestMethod]
        [ScenarioDescription("Uuid created from guidString should return the same Guid in Uuid.AsGuid as the Guid created from this guiString")]
        public void Uuid_created_from_guidString_should_return_the_same_Guid_in_AsGuid_as_the_Guid_created_from_this_guiString()
        {
            Given("a new guidString",
                context => context.State.guidString = Guid.NewGuid().ToString())
            .And("a new Guid created from guidString",
                context => context.State.guid = new Guid((string)(context.State.guidString)))
            .And("a Uuid constructed from guidString",
                context => context.State.uuid = new Uuid((string)(context.State.guidString)))
            .When("both will be compared as Guid",
                context => context.State.result = (((Uuid)context.State.uuid).AsGuid == ((Guid)context.State.guid)))
            .Then("they should be equal", context => context.State.result);
        }

        [TestMethod]
        [ScenarioDescription("2 Int64 to Uuid")]
        public void Int32_to_Uuid()
        {
            Int64 value1 = 0;
            Int64 value2 = 0;
            var uuid = new Uuid();
            Given("a first Int64 value: '54'", context => value1 = 54)
            .And("a second Int64 value: '6841266'", context => value2 = 6841266)
            .And("an empty Uuid", context => uuid = Uuid.Empty())
            .When("the first value will be set as first long L1 of the empty Uuid", context => uuid.AsLong.L1 = value1)
            .And("the second value will be set as second long L2 of the empty Uuid", context => uuid.AsLong.L2 = value2)
            .Then("the Uuid as string should be '00000036-0000-0000-b263-680000000000'", 
                context => (uuid.ToString().Equals("00000036-0000-0000-b263-680000000000", StringComparison.OrdinalIgnoreCase)));
        }
    }
}
