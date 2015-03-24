using System;
using System.Threading;
using System.Threading.Tasks;
using MS.Infrastructure;

namespace MS.EventSourcing.Infrastructure.CommandHandling
{
    public class InMemoryCommandBus: ICommandBus
    {
        readonly IEventAggregator _eventAggregator = new EventAggregator();
        
        public Task<CommandResult> Send<TRequest>(TRequest command, Action<CommandResult> handleResult) where TRequest : class
        {
            var task = Task.Factory.StartNew(() =>
            {
                CommandResult result;
                var handled = _eventAggregator.Query(command, out result);
                if (!handled)
                {
                    throw new TimeoutException("Unhandled request!");
                }

                if (handleResult != null)
                {
                    handleResult(result);
                }
                return result;
            });
            return task;
        }

        public Task<CommandResult> Send<TRequest>(TRequest command, Action<CommandResult> handleResult, TimeSpan timeout) where TRequest : class
        {
            var cancel = new CancellationTokenSource(timeout);
            var task = Task.Factory.StartNew(() =>
            {
                CommandResult result;
                var handled = _eventAggregator.Query(command, out result);
                if (!handled)
                {
                    throw new TimeoutException("Unhandled request!");
                }

                if (handleResult != null)
                {
                    handleResult(result);
                }
                return result;
            }, cancel.Token);
            return task;
        }
    }
}
