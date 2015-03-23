using MS.EventSourcing.Infrastructure.EventHandling;

namespace MS.EventSourcing.Infrastructure.Domain
{
    public interface IApplyDomainEntityEvent<in TDomainEntityEvent> where TDomainEntityEvent : DomainEntityEvent
    {
        void Apply(TDomainEntityEvent @domainEntityEvent, bool isNew);
    }
}
