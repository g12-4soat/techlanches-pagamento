namespace TechLanches.Pagamento.Core
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}
