using System;
using System.Threading.Tasks;
using Magnum.Extensions;
using MassTransit;
using MS.EventSourcing.Infrastructure.CommandHandling;

namespace MS.EventSourcing.Infrastructure.MassTransit
{
    public class CommandBus : Bus, ICommandBus
    {
        public Task<CommandResult> SendAsync<T>(T command, Action<CommandResult> handleResult) where T : class
        {
            return SendAsyncInternal(command, handleResult, 120.Seconds(), false);
        }

        public Task<CommandResult> SendAsyncWithTimeout<T>(T command, Action<CommandResult> handleResult, TimeSpan timeout) where T : class
        {
            return SendAsyncInternal(command, handleResult, timeout, true);
        }


        private Task<CommandResult> SendAsyncInternal<T>(T command, Action<CommandResult> handleResult, TimeSpan timeout, bool setRequestExpiration) where T : class
        {
            var source = new TaskCompletionSource<CommandResult>();
            ServiceBus.BeginPublishRequest(
                command,
                ar =>
                {
                    try
                    {
                        ServiceBus.EndPublishRequest<T>(ar);
                    }
                    catch (Exception ex)
                    {
                        var result = ex.ToCommandResult();
                        if (handleResult != null) handleResult(result);
                        source.SetException(ex);
                    }
                },
                null,
                cfg =>
                {
                    cfg.UseCurrentSynchronizationContext();
                    cfg.Handle<CommandResult>(commandResult =>
                    {
                        handleResult(commandResult);
                        source.SetResult(commandResult);
                    });
                    cfg.HandleFault(fault =>
                    {
                        var result = new CommandResult { Success = false, TimeStamp = fault.OccurredAt };
                        foreach (var faultMessage in fault.Messages)
                        {
                            result.Errors.Add(faultMessage);
                        }
                        foreach (var stackTraceItem in fault.StackTrace)
                        {
                            result.ErrorStackTrace.Add(stackTraceItem);
                        }
                        result.ErrorType = fault.FaultType;
                        if (handleResult != null) handleResult(result);
                        source.TrySetResult(result);
                    });
                    cfg.HandleTimeout(timeout, () =>
                    {
                        var result = new CommandResult(true, SR.ErrorMsgTaskCancelledDueToTimeout)
                        {
                            ErrorType = typeof(TimeoutException).FullName
                        };
                        if (handleResult != null) handleResult(result);
                        source.TrySetCanceled();
                    });
                    cfg.SetTimeout(timeout);
                    if (setRequestExpiration) cfg.SetRequestExpiration(timeout);
                });
            return source.Task;
        }


        protected virtual Task<CommandResult> SendCommand(object command, Action<CommandResult> handleResult)
        {
            var commandType = command.GetType();

            var method = GetGenericMethodWithCaching("SendAsync", GetType(), commandType);

            return (Task<CommandResult>)method.Invoke(this, new[] { command, handleResult });
        }

        protected virtual Task<CommandResult> SendCommand(object command, Action<CommandResult> handleResult, TimeSpan timeout)
        {
            var commandType = command.GetType();

            var method = GetGenericMethodWithCaching("SendAsyncWithTimeout", GetType(), commandType);

            return (Task<CommandResult>)method.Invoke(this, new[] { command, handleResult, timeout });
        }

        Task<CommandResult> CommandHandling.ICommandBus.Send<TRequest>(TRequest command, Action<CommandResult> handleResult)
        {
            return SendAsync(command, handleResult);
        }

        Task<CommandResult> CommandHandling.ICommandBus.Send<TRequest>(TRequest command, Action<CommandResult> handleResult, TimeSpan timeout)
        {
            return SendAsyncWithTimeout(command, handleResult, timeout);
        }
    }
}
