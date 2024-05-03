using NSubstitute;
using TechLanches.Pagamento.Adapter.ACL.QrCode.DTOs;
using TechLanches.Pagamento.Application.Gateways.Interfaces;
using TechLanches.Pagamento.Application.UseCases.Pagamentos;
using TechLanches.Pagamento.Core;
using TechLanches.Pagamento.Domain.Enums;

namespace TechLanches.Pagamento.UnitTests.UnitTests.Application.UseCases
{
    public class PagamentoUseCaseTest
    {
        [Fact]
        public async Task Cadastrar_DeveCadastrarPagamentoQuandoNaoExistente()
        {
            // Arrange
            var pedidoId = 1;
            var formaPagamento = FormaPagamento.QrCodeMercadoPago;
            var valor = 100.0m;
            var pagamentoGateway = Substitute.For<IPagamentoGateway>();

            var pagamento = new Pagamento.Domain.Aggregates.Pagamento(pedidoId, valor, formaPagamento);

            pagamentoGateway.BuscarPagamentoPorPedidoId(pedidoId).Returns(Task.FromResult<Pagamento.Domain.Aggregates.Pagamento>(null));
            pagamentoGateway.Cadastrar(Arg.Any<Pagamento.Domain.Aggregates.Pagamento>()).Returns(pagamento);

            // Act
            var pagamentoResult = await PagamentoUseCase.Cadastrar(pedidoId, formaPagamento, valor, pagamentoGateway);

            // Assert
            await pagamentoGateway.Received(1).Cadastrar(Arg.Any<Pagamento.Domain.Aggregates.Pagamento>());
            Assert.Equal(pedidoId, pagamentoResult.PedidoId);
            Assert.Equal(valor, pagamentoResult.Valor);
            Assert.Equal(formaPagamento, pagamentoResult.FormaPagamento);
        }

        [Fact]
        public async Task Cadastrar_DeveLancarExcecaoQuandoPagamentoExistente()
        {
            // Arrange
            var pedidoId = 1;
            var formaPagamento = FormaPagamento.QrCodeMercadoPago;
            var valor = 100.0m;
            var pagamentoExistente = new Pagamento.Domain.Aggregates.Pagamento(pedidoId, valor, formaPagamento);
            var pagamentoGateway = Substitute.For<IPagamentoGateway>();

            pagamentoGateway.BuscarPagamentoPorPedidoId(pedidoId).Returns(Task.FromResult(pagamentoExistente));

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(async () =>
            {
                await PagamentoUseCase.Cadastrar(pedidoId, formaPagamento, valor, pagamentoGateway);
            });
        }


        [Fact]
        public async Task RealizarPagamento_DeveAprovarPagamentoQuandoStatusAprovado()
        {
            // Arrange
            var pedidoId = 1;
            var statusPagamento = StatusPagamentoEnum.Aprovado;
            var pagamento = new Pagamento.Domain.Aggregates.Pagamento(pedidoId, 100.0m, FormaPagamento.QrCodeMercadoPago);
            var pagamentoGateway = Substitute.For<IPagamentoGateway>();

            pagamentoGateway.BuscarPagamentoPorPedidoId(pedidoId).Returns(Task.FromResult(pagamento));

            // Act
            var pagamentoAtualizado = await PagamentoUseCase.RealizarPagamento(pedidoId, statusPagamento, pagamentoGateway);

            // Assert
            Assert.Equal(StatusPagamento.Aprovado, pagamentoAtualizado.StatusPagamento);
        }

        [Fact]
        public async Task RealizarPagamento_DeveReprovarPagamentoQuandoStatusReprovado()
        {
            // Arrange
            var pedidoId = 1;
            var statusPagamento = StatusPagamentoEnum.Reprovado;
            var pagamento = new Pagamento.Domain.Aggregates.Pagamento(pedidoId, 100.0m, FormaPagamento.QrCodeMercadoPago);
            var pagamentoGateway = Substitute.For<IPagamentoGateway>();

            pagamentoGateway.BuscarPagamentoPorPedidoId(pedidoId).Returns(Task.FromResult(pagamento));

            // Act
            var pagamentoAtualizado = await PagamentoUseCase.RealizarPagamento(pedidoId, statusPagamento, pagamentoGateway);

            // Assert
            Assert.Equal(StatusPagamento.Reprovado, pagamentoAtualizado.StatusPagamento);
        }

        [Fact]
        public async Task RealizarPagamento_DeveLancarExcecaoQuandoPagamentoNaoExistente()
        {
            // Arrange
            var pedidoId = 1;
            var statusPagamento = StatusPagamentoEnum.Aprovado;
            var pagamentoGateway = Substitute.For<IPagamentoGateway>();

            pagamentoGateway.BuscarPagamentoPorPedidoId(pedidoId).Returns(Task.FromResult<Pagamento.Domain.Aggregates.Pagamento>(null));

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(async () =>
            {
                await PagamentoUseCase.RealizarPagamento(pedidoId, statusPagamento, pagamentoGateway);
            });
        }

        [Fact]
        public async Task RealizarPagamento_DeveLancarExcecaoAoRepetirPagamentoAprovado()
        {
            // Arrange
            var pedidoId = 1;
            var pagamentoGateway = Substitute.For<IPagamentoGateway>();
            var statusPagamento = StatusPagamentoEnum.Aprovado;
            var pagamento = new Pagamento.Domain.Aggregates.Pagamento(pedidoId, 100.0m, FormaPagamento.QrCodeMercadoPago);

            pagamento.Aprovar();

            pagamentoGateway.BuscarPagamentoPorPedidoId(pedidoId).Returns(Task.FromResult(pagamento));

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(async () =>
            {
                await PagamentoUseCase.RealizarPagamento(pedidoId, statusPagamento, pagamentoGateway);
            });
        }
    }
}
