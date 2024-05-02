using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using System.Text;
using System.Text.Json;
using TechLanches.Pagamento.Application.Constantes;
using TechLanches.Pagamento.Application.Controllers;
using TechLanches.Pagamento.Application.DTOs;
using TechLanches.Pagamento.Application.Gateways;
using TechLanches.Pagamento.Application.Gateways.Interfaces;
using TechLanches.Pagamento.Domain.Enums.Pedido;
using TechLanches.Pagamento.UnitTests.FakeHttpHandler;

namespace TechLanches.Pagamento.UnitTests.UnitTests.Application.Controllers
{
    public class PedidoControllerTest
    {
        private readonly IPedidoGateway _pedidoGateway;
        private readonly PedidoController _pedidoController;
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;
        public PedidoControllerTest()
        {
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            var pedidoResponseJsonMock = JsonSerializer.Serialize(new PedidoResponseDTO { Id = 1 });
            var handler = new FakeHttpMessageHandler(pedidoResponseJsonMock, System.Net.HttpStatusCode.OK);
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://example.com/") 
            };
            httpClientFactory.CreateClient(Constantes.API_PEDIDO).Returns(_httpClient);
            _cache = new MemoryCache(new MemoryCacheOptions());
            _pedidoGateway = new PedidoGateway(httpClientFactory, _cache);
            _pedidoController = new PedidoController(httpClientFactory, _cache);
        }

        [Fact]
        public async Task TrocarStatus_Pedido_DeveRetornarPedidoResponseDto()
        {
            // Arrange
            var pedidoId = 1;
            var pedidoResponseDto = new PedidoResponseDTO { Id = pedidoId };

            _cache.Set("authtoken", "bearer token");

            // Act
            var result = await _pedidoController.TrocarStatus(pedidoId, StatusPedido.PedidoRecebido);

            // Assert
            Assert.Equal(pedidoResponseDto.Id, result.Id);
        }
    }
    
}
