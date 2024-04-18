using TechLanches.Pagamento.Core;

namespace TechLanches.Pagamento.Application.Ports.Repositories
{
    public interface IPagamentoRepository : IRepository<Domain.Aggregates.Pagamento>
    {
        Task<Domain.Aggregates.Pagamento> BuscarPagamentoPorPedidoId(int pedidoId);
        Task<Domain.Aggregates.Pagamento> Cadastrar(Domain.Aggregates.Pagamento pagamento);
    }
}