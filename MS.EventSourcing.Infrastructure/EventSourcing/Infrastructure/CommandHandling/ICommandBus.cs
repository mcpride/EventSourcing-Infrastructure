using System;
using System.Threading.Tasks;

namespace MS.EventSourcing.Infrastructure.CommandHandling
{
    /// <summary>
    /// Represents a message bus from which commands are broadcast to handlers
    /// </summary>
    public interface ICommandBus
    {
        Task<CommandResult> Send<TRequest>(TRequest command, Action<CommandResult> handleResult) where TRequest : class;
        Task<CommandResult> Send<TRequest>(TRequest command, Action<CommandResult> handleResult, TimeSpan timeout) where TRequest : class;
    }
}