namespace MS.EventSourcing.Infrastructure.MassTransit
{
    public interface ICommandBus : CommandHandling.ICommandBus, IBusInitialize
    {
    }
}
