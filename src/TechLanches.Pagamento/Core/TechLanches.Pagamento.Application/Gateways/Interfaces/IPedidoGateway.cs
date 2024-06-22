using TechLanches.Pagamento.Application.DTOs;
using TechLanches.Pagamento.Domain.Enums.Pedido;

namespace TechLanches.Pagamento.Application.Gateways.Interfaces
{
    public interface IPedidoGateway
    {
        Task<PedidoResponseDTO> TrocarStatus(int pedidoId, StatusPedido statusPedido);
        Task<List<PedidoResponseDTO>> BuscarPedidosPorCpf(string cpf);
    }
}