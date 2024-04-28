using TechLanches.Pagamento.Application.DTOs;
using TechLanches.Pagamento.Domain.Enums.Pedido;

namespace TechLanches.Pagamento.Application.Controllers
{
    public interface IPedidoController
    {
        Task<PedidoResponseDTO> TrocarStatus(int pedidoId, StatusPedido statusPedido);
    }
}
