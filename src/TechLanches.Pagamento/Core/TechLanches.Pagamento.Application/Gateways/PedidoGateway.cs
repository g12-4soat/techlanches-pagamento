using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using TechLanches.Pagamento.Application.DTOs;
using TechLanches.Pagamento.Application.Gateways.Interfaces;
using TechLanches.Pagamento.Core;
using TechLanches.Pagamento.Domain.Enums.Pedido;
using Microsoft.Extensions.Caching.Memory;

namespace TechLanches.Pagamento.Application.Gateways
{
    public class PedidoGateway : IPedidoGateway
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        public PedidoGateway(IHttpClientFactory httpClientFactory, IMemoryCache cache)
        {
            _cache = cache;
            _httpClient = httpClientFactory.CreateClient(Constantes.Constantes.API_PEDIDO);
        }

        public async Task<PedidoResponseDTO> TrocarStatus(int pedidoId, StatusPedido statusPedido)
        {
            var token = _cache.Get("authtoken").ToString().Split(" ")[1];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);

            var content = new StringContent(((int)statusPedido).ToString(), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/pedidos/{pedidoId}/trocarstatus", content);

            if (response.IsSuccessStatusCode == false)
                throw new Exception("Erro durante chamada api de pedidos.");

            string resultStr = await response.Content.ReadAsStringAsync();

            var pedido = JsonSerializer.Deserialize<PedidoResponseDTO>(resultStr);

            return pedido;
        }
    }
}