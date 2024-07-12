using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Net;
using System.Text.Json;
using TechLanches.Pagamento.Application.Constantes;
using TechLanches.Pagamento.Application.DTOs;
using TechLanches.Pagamento.Application.Gateways;
using TechLanches.Pagamento.Domain.Enums.Pedido;
using TechLanches.Pagamento.UnitTests.FakeHttpHandler;

namespace TechLanches.Pagamento.UnitTests.UnitTests.Application.Gateways
{
    public class PedidoGatewayTest
    {
        private readonly IMemoryCache _cache;
        private HttpClient _httpClient;
        private PedidoGateway _pedidoGateway;
        public PedidoGatewayTest()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        [Fact]
        public async Task TrocarStatus_DeveChamarMetodosCorretosETratarStatusCodeOk()
        {
            // Arrange
            var pedidoId = 1;
            var statusPedido = StatusPedido.PedidoRecebido;
            var pedidoResponseDto = new PedidoResponseDTO { Id = pedidoId, StatusPedido = statusPedido };

            _cache.Set("authtoken", "bearer token");
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            var pedidoResponseJsonMock = JsonSerializer.Serialize(pedidoResponseDto);
            var logger = Substitute.For<ILogger<PedidoGateway>>();
            var handler = new FakeHttpMessageHandler(pedidoResponseJsonMock, System.Net.HttpStatusCode.OK);
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://example.com/")
            };
            httpClientFactory.CreateClient(Constantes.API_PEDIDO).Returns(_httpClient);
            _pedidoGateway = new PedidoGateway(httpClientFactory, _cache, logger);

            // Act
            var pedidoResponse = await _pedidoGateway.TrocarStatus(pedidoId, statusPedido);

            // Assert
            Assert.Equal(pedidoId, pedidoResponse.Id);
            Assert.Equal(statusPedido, pedidoResponse.StatusPedido);
        }
    }
}
