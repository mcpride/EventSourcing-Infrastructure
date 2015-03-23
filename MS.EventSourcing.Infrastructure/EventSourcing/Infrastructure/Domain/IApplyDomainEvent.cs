using MS.EventSourcing.Infrastructure.EventHandling;

namespace MS.EventSourcing.Infrastructure.Domain
{
    public interface IApplyDomainEvent<in TDomainEvent> where TDomainEvent : DomainEvent
    {
        void Apply(TDomainEvent @domainEvent, bool isNew);
    }
}
