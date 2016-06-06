namespace MS.EventSourcing.Infrastructure.EventHandling
{
    /// <summary>
    /// Markes an domain event as external event.
    /// Such an external event has no relevance for event sourcing and will be broadcasted only.
    /// </summary>
    public interface IExternalEvent // marker interface
    {
    }
}
