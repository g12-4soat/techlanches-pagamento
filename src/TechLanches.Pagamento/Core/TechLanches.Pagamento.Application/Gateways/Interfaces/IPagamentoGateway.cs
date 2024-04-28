using TechLanches.Pagamento.Adapter.ACL.QrCode.DTOs;

namespace TechLanches.Pagamento.Application.Gateways.Interfaces
{
    public interface IPagamentoGateway
    {
        Task<Domain.Aggregates.Pagamento> BuscarPagamentoPorPedidoId(int pedidoId);
        Task<Domain.Aggregates.Pagamento> Cadastrar(Domain.Aggregates.Pagamento pagamento);
        Task<Domain.Aggregates.Pagamento> Atualizar(Domain.Aggregates.Pagamento pagamento);
        Task<string> GerarPagamentoEQrCode();
        Task<PagamentoResponseACLDTO> ConsultarPagamento(string pedidoComercial);
    }
}
