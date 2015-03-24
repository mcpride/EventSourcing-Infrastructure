using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MS.Infrastructure
{
    public delegate bool ResponseDelegate(out object resp);
    public delegate bool ResponseDelegate<TResponse>(out TResponse resp);
    
    /// <summary>
    /// Enables loosely-coupled publication of and subscription to events.
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        public static class Factory
        {
            private static IEventAggregator _globalInstance;
            public static Func<IEventAggregator> NewInstance = () => new EventAggregator();
            public static Func<IEventAggregator> GlobalInstance = () => _globalInstance ?? (_globalInstance = new EventAggregator());
        }
        
        readonly List<Handler> _handlers = new List<Handler>();


        public EventAggregator() { }
        
        /// <summary>
        /// Searches the subscribed handlers to check if we have a handler for
        /// the message type supplied.
        /// </summary>
        /// <param name="messageType">The message type to check with</param>
        /// <returns>True if any handler is found, false if not.</returns>
        public bool HandlerExistsFor(Type messageType)
        {
            return _handlers.Any(handler => handler.Handles(messageType) & !handler.IsDead);
        }

        /// <summary>
        /// Subscribes an instance to all events declared through implementations of <see cref = "IHandle{T}" />
        /// </summary>
        /// <param name = "subscriber">The instance to subscribe for event publication.</param>
        public virtual void Subscribe(object subscriber)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException("subscriber");
            }
            lock (_handlers)
            {
                if (_handlers.Any(x => x.Matches(subscriber)))
                {
                    return;
                }

                _handlers.Add(new Handler(subscriber));
            }
        }

        /// <summary>
        /// Unsubscribes the instance from all events.
        /// </summary>
        /// <param name = "subscriber">The instance to unsubscribe.</param>
        public virtual void Unsubscribe(object subscriber)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException("subscriber");
            }
            lock (_handlers)
            {
                var found = _handlers.FirstOrDefault(x => x.Matches(subscriber));

                if (found != null)
                {
                    _handlers.Remove(found);
                }
            }
        }

        /// <summary>
        /// Unsubscribes from all events.
        /// </summary>
        public virtual void UnsubscribeAll()
        {
            lock (_handlers)
            {
                _handlers.Clear();
            }
        }

        /// <summary>
        /// Publishes a message.
        /// </summary>
        /// <param name = "message">The message instance.</param>
        /// <param name = "marshal">Allows the publisher to provide a custom thread marshaller for the message publication.</param>
        public virtual void Publish(object message, Action<Action> marshal)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (marshal == null)
            {
                throw new ArgumentNullException("marshal");
            }

            Handler[] toNotify;
            lock (_handlers)
            {
                toNotify = _handlers.ToArray();
            }

            marshal(() =>
            {
                var messageType = message.GetType();

                var dead = toNotify
                    .Where(handler => !handler.Handle(messageType, message))
                    .ToList();

                if (dead.Any())
                {
                    lock (_handlers)
                    {
                        dead.Apply(x => _handlers.Remove(x));
                    }
                }
            });
        }

        /// <summary>
        /// Executes a query for a message.
        /// </summary>
        /// <param name = "message">The message instance.</param>
        /// <param name="response"></param>
        public virtual bool Query<TResponse>(object message, out TResponse response)
        {
            response = default(TResponse);
            var handled = false;
            if (message == null) throw new ArgumentNullException("message");

            Handler[] toNotify;
            lock (_handlers)
            {
                toNotify = _handlers.ToArray();
            }

            var messageType = message.GetType();

            var dead = new List<Handler>();

            foreach (var handler in toNotify)
            {
                object result;
                bool isHandled;
                if (!handler.HandleWithResponse(messageType, typeof(TResponse), message, out result, out isHandled)) dead.Add(handler);
                if (!isHandled) continue;
                handled = true;
                response = (TResponse)result;
                break;
            }

            if (dead.Any())
            {
                lock (_handlers)
                {
                    dead.Apply(x => _handlers.Remove(x));
                }
            }

            return handled;
        }

        class Handler
        {
            readonly WeakReference _reference;
            readonly Dictionary<Type, MethodInfo> _supportedHandlers = new Dictionary<Type, MethodInfo>();
            readonly Dictionary<Type, Dictionary<Type, MethodInfo>> _supportedResponseHandlers = new Dictionary<Type, Dictionary<Type, MethodInfo>>();

            public bool IsDead
            {
                get { return _reference.Target == null; }
            }

            public Handler(object handler)
            {
                _reference = new WeakReference(handler);

                var interfaces = handler.GetType().GetInterfaces()
                    .Where(x => typeof(IHandle).IsAssignableFrom(x) && x.IsGenericType);

                foreach (var @interface in interfaces)
                {
                    var types = @interface.GetGenericArguments();
                    if (types.Length == 1)
                    {
                        var method = @interface.GetMethod("Handle");
                        _supportedHandlers[types[0]] = method;
                    }
                    else
                    {
                        var method = @interface.GetMethod("Respond");
                        if (!_supportedResponseHandlers.ContainsKey(types[0]))
                        {
                            _supportedResponseHandlers[types[0]] = new Dictionary<Type, MethodInfo>();
                        }
                        _supportedResponseHandlers[types[0]][types[1]] = method;
                    }
                }
            }

            public bool Matches(object instance)
            {
                return _reference.Target == instance;
            }

            public bool Handle(Type messageType, object message)
            {
                var target = _reference.Target;
                if (target == null)
                {
                    return false;
                }

                foreach (var pair in _supportedHandlers.Where(pair => pair.Key.IsAssignableFrom(messageType)))
                {
                    pair.Value.Invoke(target, new[] {message});
                }

                return true;
            }

            public bool HandleWithResponse(Type messageType, Type returnType, object message, out object returnValue, out bool handled)
            {
                returnValue = null;
                handled = false;
                var target = _reference.Target;
                if (target == null)
                {
                    return false;
                }

                foreach (var dictPair in _supportedResponseHandlers)
                {
                    if (!dictPair.Key.IsAssignableFrom(messageType)) continue;
                    foreach (var pair in dictPair.Value)
                    {
                        if (!dictPair.Key.IsAssignableFrom(returnType)) continue;
                        var args = new[] { message, null, false };
                        pair.Value.Invoke(target, args);
                        handled = (bool) args[2];
                        if (!handled) continue;
                        returnValue = args[1];
                        return true;
                    }
                }

                return true;
            }

            public bool Handles(Type messageType)
            {
                return _supportedHandlers.Any(pair => pair.Key.IsAssignableFrom(messageType));
            }
        }
    }
}