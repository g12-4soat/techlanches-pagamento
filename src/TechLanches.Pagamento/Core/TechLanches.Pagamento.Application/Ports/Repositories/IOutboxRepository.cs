using TechLanches.Pagamento.Domain.Aggregates;

namespace TechLanches.Pagamento.Application.Ports.Repositories
{
    public interface IOutboxRepository
    {
        Task<IEnumerable<Outbox>> ObterNaoProcessados();
        Task ProcessarMensagem(Outbox message);
    }
}