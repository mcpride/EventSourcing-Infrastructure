namespace MS.EventSourcing.Infrastructure.MassTransit
{
    public interface IEventBus : EventHandling.IEventBus, IBusInitialize
    {
    }
}
