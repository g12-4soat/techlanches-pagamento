using NSubstitute;
using TechLanches.Pagamento.Adapter.ACL.QrCode.DTOs;
using TechLanches.Pagamento.Adapter.ACL.QrCode.Provedores.MercadoPago;
using TechLanches.Pagamento.Application.Gateways;
using TechLanches.Pagamento.Application.Ports.Repositories;
using TechLanches.Pagamento.Domain.Enums;

namespace TechLanches.Pagamento.UnitTests.UnitTests.Application.Gateways
{
    public class PagamentoGatewayTests
    {
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly IMercadoPagoMockadoService _mercadoPagoMockadoService;
        private readonly PagamentoGateway _pagamentoGateway;

        public PagamentoGatewayTests()
        {
            _pagamentoRepository = Substitute.For<IPagamentoRepository>();
            _mercadoPagoMockadoService = Substitute.For<IMercadoPagoMockadoService>();
            _pagamentoGateway = new PagamentoGateway(_pagamentoRepository, _mercadoPagoMockadoService);
        }

        [Fact]
        public async Task Atualizar_DeveChamarMetodoCorretoDoRepositorio()
        {
            // Arrange
            var pagamento = new Pagamento.Domain.Aggregates.Pagamento(1, 100.0m, FormaPagamento.QrCodeMercadoPago);

            // Act
            await _pagamentoGateway.Atualizar(pagamento);

            // Assert
            await _pagamentoRepository.Received(1).Atualizar(pagamento);
        }

        [Fact]
        public async Task BuscarPagamentoPorPedidoId_DeveObterPedidoComSucesso()
        {
            // Arrange
            var pedidoId = 1;
            var pagamento = new Pagamento.Domain.Aggregates.Pagamento(pedidoId, 100.0m, FormaPagamento.QrCodeMercadoPago);
            _pagamentoRepository.BuscarPagamentoPorPedidoId(pedidoId).Returns(pagamento);
            // Act
            await _pagamentoGateway.BuscarPagamentoPorPedidoId(pedidoId);

            // Assert
            Assert.Equal(pedidoId, pagamento.PedidoId);
            await _pagamentoRepository.Received(1).BuscarPagamentoPorPedidoId(pedidoId);
        }

        [Fact]
        public async Task Cadastrar_DeveChamarMetodoCorretoDoRepositorio()
        {
            // Arrange
            var pagamento = new Pagamento.Domain.Aggregates.Pagamento(1, 100.0m, FormaPagamento.QrCodeMercadoPago);

            // Act
            await _pagamentoGateway.Cadastrar(pagamento);

            // Assert
            await _pagamentoRepository.Received(1).Cadastrar(pagamento);
        }

        [Fact]
        public async Task ConsultarPagamento_DeveRetornarPagamentoComSucesso()
        {
            // Arrange
            var pedidoComercial = "1";
            _mercadoPagoMockadoService.ConsultarPagamento(pedidoComercial).Returns(new PagamentoResponseACLDTO());

            // Act
            var pagamento = await _pagamentoGateway.ConsultarPagamento(pedidoComercial);

            // Assert
            Assert.NotNull(pagamento);
            Assert.IsType<PagamentoResponseACLDTO>(pagamento);
            await _mercadoPagoMockadoService.Received(1).ConsultarPagamento(pedidoComercial);
        }

        [Fact]
        public async Task GerarPagamentoEQrCode_DeveRetornarQrCode()
        {
            _mercadoPagoMockadoService.GerarPagamentoEQrCode().Returns("qrcodedata");

            // Act
            var qrCode = await _pagamentoGateway.GerarPagamentoEQrCode();

            // Assert
            Assert.NotNull(qrCode);
            await _mercadoPagoMockadoService.Received(1).GerarPagamentoEQrCode();
        }
    }
}
