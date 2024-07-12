using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TechLanches.Pagamento.Application.DTOs;
using TechLanches.Pagamento.Application.Gateways.Interfaces;
using TechLanches.Pagamento.Domain.Enums.Pedido;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace TechLanches.Pagamento.Application.Gateways
{
    public class PedidoGateway : IPedidoGateway
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<PedidoGateway> _logger;
        public PedidoGateway(IHttpClientFactory httpClientFactory, IMemoryCache cache, ILogger<PedidoGateway> logger)
        {
            _cache = cache;
            _httpClient = httpClientFactory.CreateClient(Constantes.Constantes.API_PEDIDO);
            _logger = logger;
        }

        public async Task<PedidoResponseDTO> TrocarStatus(int pedidoId, StatusPedido statusPedido)
        {
            SetAuthToken();

            var content = new StringContent(((int)statusPedido).ToString(), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/pedidos/{pedidoId}/trocarstatus", content);

            LogResponse(response);

            string resultStr = await response.Content.ReadAsStringAsync();

            var pedido = JsonSerializer.Deserialize<PedidoResponseDTO>(resultStr);

            return pedido;
        }

        public async Task<List<PedidoResponseDTO>> BuscarPedidosPorCpf(string cpf)
        {
            SetAuthToken();

            var response = await _httpClient.GetAsync($"api/pedidos/{cpf}");
            
            LogResponse(response);

            var pedidos = new List<PedidoResponseDTO>();

            if (response.IsSuccessStatusCode)
            {
                string resultStr = await response.Content.ReadAsStringAsync();

                pedidos = JsonSerializer.Deserialize<List<PedidoResponseDTO>>(resultStr);
            }

            return pedidos;
        }

        private void LogResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Erro durante chamada api de pedidos. Status Code:{StatusCode}. Response: {response}.", response.StatusCode, response.Content?.ReadAsStringAsync());
                return;
            }

            _logger.LogInformation($"Sucesso na chamada da api de pedidos.");
        }

        private void SetAuthToken()
        {
            var token = _cache.Get("authtoken").ToString().Split(" ")[1];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}