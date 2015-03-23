using System;
using MassTransit;
using MassTransit.BusConfigurators;
using MassTransit.SubscriptionConfigurators;
using MassTransit.Transports.RabbitMq.Configuration.Configurators;

namespace MS.EventSourcing.Infrastructure.MassTransit
{
    public interface IBusInitialize : IDisposable
    {
        IServiceBus Initialize(Action<SubscriptionBusServiceConfigurator> subscribe, Action<ServiceBusConfigurator> configure);
        IServiceBus InitializeMsmq(string queueName, Action<SubscriptionBusServiceConfigurator> subscribe, Uri subscriptionServiceUri = null);
        IServiceBus InitializeRabbitMq(string queueName, Action<SubscriptionBusServiceConfigurator> subscribe, Uri hostUri, Action<ConnectionFactoryConfigurator> configureHost);
        void InitializeWithServiceBus(IServiceBus serviceBus);
    }
}
