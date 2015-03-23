using System;
using System.Threading.Tasks;
using MS.EventSourcing.Infrastructure.CommandHandling;
using MS.EventSourcing.Infrastructure.MassTransit;

namespace MS.EventSourcing.Infrastructure.UnitTests
{
    public class TestCommandBus : CommandBus
    {
        public new Task<CommandResult> SendCommand(object command, Action<CommandResult> handleResult)
        {
            return base.SendCommand(command, handleResult);
        }

        public new Task<CommandResult> SendCommand(object command, Action<CommandResult> handleResult, TimeSpan timeout)
        {
            return base.SendCommand(command, handleResult, timeout);
        }
    }
}
