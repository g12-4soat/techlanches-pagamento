using TechLanches.Pagamento.Adapter.ACL.QrCode.DTOs;

namespace TechLanches.Pagamento.Application.Gateways.Interfaces
{
    public interface IPagamentoGateway : IRepositoryGateway
    {
        Task<Domain.Aggregates.Pagamento> BuscarPagamentoPorPedidoId(int pedidoId);
        Task<Domain.Aggregates.Pagamento> Cadastrar(Domain.Aggregates.Pagamento pagamento);
        Task<string> GerarPagamentoEQrCodeMercadoPago(PedidoACLDTO pedidoMercadoPago);
        Task<PagamentoResponseACLDTO> ConsultarPagamentoMercadoPago(string pedidoComercial);
    }
}
