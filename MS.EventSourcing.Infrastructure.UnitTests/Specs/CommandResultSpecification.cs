using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MS.QualityTools.UnitTestFramework.Specifications;
using MS.EventSourcing.Infrastructure.CommandHandling;

namespace MS.EventSourcing.Infrastructure.UnitTests.Specs
{
    [TestClass]
    [SpecificationDescription("Command result specification")]
    public class CommandResultSpecification : Specification
    {
        [TestMethod]
        [ScenarioDescription("Command result creation without error has state 'Success' = true")]
        public void Command_result_creation_without_error_sets_success_state_to_true()
        {
            Given("a command result creation without error message",
                testContext => testContext.State.Commandresult = CommandResult.Successful)
            .Then("state 'Success' of this command result is 'true'",
                testContext => ((CommandResult) testContext.State.Commandresult).Success.Should().BeTrue());
        }

        [TestMethod]
        [ScenarioDescription("Command result creation with error has state 'Success' = false")]
        public void Command_result_creation_wit_error_sets_success_state_to_false()
        {
            Given("a command result creation with error message",
                testContext => testContext.State.Commandresult = new CommandResult("error"))
            .Then("state 'Success' of this command result is 'false'",
                testContext =>
                {
                    ((CommandResult) testContext.State.Commandresult).Success.Should().BeFalse();
                    ((CommandResult) testContext.State.Commandresult).Errors.Should().NotBeEmpty();
                });
        }
    }
}
