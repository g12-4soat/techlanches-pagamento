namespace TechLanches.Pagamento.Adapter.Outbox.Services
{
    public interface IOutboxService
    {
        Task ProcessMessages();
    }
}
