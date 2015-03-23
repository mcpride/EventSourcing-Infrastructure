using System;
using MassTransit;
using MS.EventSourcing.Infrastructure.CommandHandling;
using MS.EventSourcing.Infrastructure.UnitTests.Commands;

namespace MS.EventSourcing.Infrastructure.UnitTests.Handlers
{
    public class CommandBusTestConsumer: Consumes<CommandBusTestCommand>.Context
    {
        public void Consume(IConsumeContext<CommandBusTestCommand> consumeContext)
        {
            try
            {
                if (consumeContext.Message.ShouldThrowException)
                {
                    throw new InvalidOperationException("This is an expected exception!");
                }
                consumeContext.Respond(CommandResult.Successful);
            }
            catch (Exception ex)
            {
                consumeContext.Respond(ex.ToCommandResult());
            }
        }
    }
}