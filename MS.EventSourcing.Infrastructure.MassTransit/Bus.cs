using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MassTransit;
using MassTransit.BusConfigurators;
using MassTransit.SubscriptionConfigurators;
using MassTransit.Transports.RabbitMq.Configuration.Configurators;

namespace MS.EventSourcing.Infrastructure.MassTransit
{
    public abstract class Bus : IBusInitialize
    {
        protected IServiceBus ServiceBus;
        private bool _initialized;
        private static readonly IDictionary<string, MethodInfo> MethodCache = new ConcurrentDictionary<string, MethodInfo>();


        public IServiceBus InitializeMsmq(string queueName, Action<SubscriptionBusServiceConfigurator> subscribe = null, Uri subscriptionServiceUri = null)
        {
            if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException("queueName");
            return Initialize(subscribe, sbc =>
            {
                sbc.UseMsmq(mqc =>
                {
                    if (subscriptionServiceUri == null)
                    {
                        mqc.UseMulticastSubscriptionClient();
                    }
                    else
                    {
                        mqc.UseSubscriptionService(subscriptionServiceUri);
                    }
                    mqc.VerifyMsmqConfiguration();
                });
                sbc.ReceiveFrom("msmq://localhost/" + queueName);
                sbc.UseControlBus(
                    cbc => cbc.ReceiveFrom(new Uri("msmq://localhost/" + queueName + "_control")));
            });
        }

        public IServiceBus InitializeRabbitMq(string queueName, Action<SubscriptionBusServiceConfigurator> subscribe, Uri hostUri, Action<ConnectionFactoryConfigurator> configureHost)
        {
            if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException("queueName");
            return Initialize(subscribe, sbc =>
            {
                sbc.UseRabbitMq(rqc => rqc.ConfigureHost(hostUri, configureHost));
                sbc.ReceiveFrom("rabbitmq://localhost/" + queueName);
                sbc.UseControlBus(
                    cbc => cbc.ReceiveFrom(new Uri("rabbitmq://localhost/" + queueName + "_control")));
            });
        }

        public IServiceBus Initialize(Action<SubscriptionBusServiceConfigurator> subscribe, Action<ServiceBusConfigurator> configure)
        {
            if (_initialized) return ServiceBus;
            ServiceBus = ServiceBusFactory.New(sbc =>
            {
                configure(sbc);
                if (subscribe != null)
                {
                    sbc.Subscribe(subscribe);
                }
            });
            ServiceBus.Probe();
            ServiceBus.WriteIntrospectionToConsole();
            _initialized = true;
            return ServiceBus;
        }

        public void InitializeWithServiceBus(IServiceBus serviceBus)
        {
            if (serviceBus == null) throw new ArgumentNullException("serviceBus");
            if (_initialized) return;
            ServiceBus = serviceBus;
            _initialized = true;
        }

        protected static MethodInfo GetGenericMethodWithCaching(string name, Type methodType, Type commandType)
        {
            MethodInfo method;

            var localKey = HashString(string.Format("{0}{1}{2}", methodType.AssemblyQualifiedName, name, commandType.AssemblyQualifiedName));

            if (MethodCache.ContainsKey(localKey))
            {
                method = MethodCache[localKey];
            }
            else
            {
                var methodInfo = methodType.GetMethod(name);
                method = methodInfo.MakeGenericMethod(commandType);
                MethodCache[localKey] = method;
            }
            return method;
        }

        private static string HashString(string value)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var retVal = md5.ComputeHash(Encoding.UTF8.GetBytes(value));
                var sb = new StringBuilder();
                foreach (var b in retVal)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        #region Disposable pattern

        private bool _disposed;

        ~Bus()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
                if (ServiceBus != null) ServiceBus.Dispose();
            }

            // release any unmanaged objects
            // set thick object references to null
            ServiceBus = null;
            _disposed = true;
        }

        #endregion Disposable pattern
    }
}
