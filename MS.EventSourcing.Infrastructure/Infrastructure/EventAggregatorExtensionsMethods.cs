using System.Threading.Tasks;

namespace MS.Infrastructure
{
    /// <summary>
    /// Extensions for <see cref="IEventAggregator"/>.
    /// </summary>
    public static class EventAggregatorExtensionsMethods
    {
        /// <summary>
        /// Publishes a message on the current thread (synchrone).
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name = "message">The message instance.</param>
        public static void PublishOnCurrentThread(this IEventAggregator eventAggregator, object message)
        {
            eventAggregator.Publish(message, action => action());
        }

        /// <summary>
        /// Publishes a message on a background thread (async).
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name = "message">The message instance.</param>
        public static void PublishOnBackgroundThread(this IEventAggregator eventAggregator, object message)
        {
            eventAggregator.Publish(message, action => Task.Factory.StartNew(action));
        }

        /// <summary>
        /// Publishes a message asynchronously.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="message">The message instance.</param>
        // ReSharper disable InconsistentNaming
        public static Task PublishAsync(this IEventAggregator eventAggregator, object message)
        // ReSharper restore InconsistentNaming
        {
            Task task = null;
            eventAggregator.Publish(message, action =>
            {
                task = Task.Factory.StartNew(action);
            });
            return task;
        }
    }
}