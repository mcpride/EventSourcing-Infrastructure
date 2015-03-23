namespace MS.EventSourcing.Infrastructure.EF
{
    public interface IRepositoryFactory
    {
        IRepository NewRepository();
    }
}