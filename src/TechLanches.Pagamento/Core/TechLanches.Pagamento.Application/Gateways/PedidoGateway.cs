using System.Text;
using System.Text.Json;
using TechLanches.Pagamento.Application.DTOs;
using TechLanches.Pagamento.Application.Gateways.Interfaces;
using TechLanches.Pagamento.Domain.Enums.Pedido;

namespace TechLanches.Pagamento.Application.Gateways
{
    public class PedidoGateway : IPedidoGateway
    {
        private readonly HttpClient _httpClient;

        public PedidoGateway(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(Constantes.Constantes.API_PEDIDO);
        }

        public async Task<PedidoResponseDTO> TrocarStatus(int pedidoId, StatusPedido statusPedido)
        {
            var content = new StringContent(((int)statusPedido).ToString(), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"api/pedidos/{pedidoId}/trocarstatus", content);

            string resultStr = await response.Content.ReadAsStringAsync();

            var pedido = JsonSerializer.Deserialize<PedidoResponseDTO>(resultStr);

            return pedido;
        }
    }
}