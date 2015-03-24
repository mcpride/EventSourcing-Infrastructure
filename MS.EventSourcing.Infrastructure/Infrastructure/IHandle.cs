using System.Runtime.InteropServices;

namespace MS.Infrastructure
{
    /// <summary>
    ///   A marker interface for classes that subscribe to messages.
    /// </summary>
    public interface IHandle
    {
    }

    /// <summary>
    ///   Denotes a class which can handle a particular type of message.
    /// </summary>
    /// <typeparam name = "TMessage">The type of message to handle.</typeparam>
// ReSharper disable TypeParameterCanBeVariant
    public interface IHandle<TMessage> : IHandle
// ReSharper restore TypeParameterCanBeVariant
    { //don't use contravariance here
        /// <summary>
        ///   Handles the message.
        /// </summary>
        /// <param name = "message">The message.</param>
        void Handle(TMessage message);
    }

    /// <summary>
    ///   Denotes a class which can handle a particular type of message.
    /// </summary>
    /// <typeparam name = "TMessage">The type of message to handle.</typeparam>
    /// <typeparam name="TResponse">The resulting out parameter.</typeparam>
    // ReSharper disable TypeParameterCanBeVariant
    public interface IHandle<TMessage, TResponse> : IHandle
    // ReSharper restore TypeParameterCanBeVariant
    { //don't use contravariance here

        /// <summary>
        /// Responses the message.
        /// </summary>
        /// <param name = "message">The message.</param>
        /// <param name="response">The response value.</param>
        /// <param name="handled"> returns true if message has been handled</param>
        void Respond(TMessage message, out TResponse response, out bool handled);
    }
}