using System.Threading;
using MassTransit;
using MS.EventSourcing.Infrastructure.UnitTests.Events;

namespace MS.EventSourcing.Infrastructure.UnitTests.Handlers
{
    public class EventBusTestConsumer : Consumes<CustomerCreated>.All, Consumes<CustomerNameChanged>.All
    {
        public static long HandledEvents = 0;

        public void Consume(CustomerCreated message)
        {
            Interlocked.Increment(ref HandledEvents);
        }

        public void Consume(CustomerNameChanged message)
        {
            Interlocked.Increment(ref HandledEvents);
        }
    }
}