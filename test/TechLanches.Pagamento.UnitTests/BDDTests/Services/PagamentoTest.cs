using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using TechLanches.Pagamento.Adapter.ACL.QrCode.Provedores.MercadoPago;
using TechLanches.Pagamento.Adapter.RabbitMq.Messaging;
using TechLanches.Pagamento.Adapter.RabbitMq.Options;
using TechLanches.Pagamento.Application.Controllers;
using TechLanches.Pagamento.Application.DTOs;
using TechLanches.Pagamento.Application.Gateways.Interfaces;
using TechLanches.Pagamento.Application.Ports.Repositories;
using TechLanches.Pagamento.Application.Presenters;
using TechLanches.Pagamento.Domain.Enums;

namespace TechLanches.Pagamento.UnitTests.BDDTests.Services
{
    [Trait("Services", "Pagamento")]
    public class PagamentoTest
    {
        private Pagamento.Domain.Aggregates.Pagamento _pagamento;
        private PagamentoResponseDTO _novoPagamentoDto;


        [Fact(DisplayName = "Deve cadastrar pagamento com sucesso")]
        public async Task CadastrarPagamento_DeveRetornarSucesso()
        {
            Given_PagamentoComDadosValidos();
            await When_CadastrarPagamento();
            Then_PagamentoCriadoNaoDeveSerNulo();
            Then_TodosAsPropriedadesDevemSerIguais();
        }


        private void Given_PagamentoComDadosValidos()
        {
            var pedidoId = 1;
            var valor = 100;

            _pagamento = new Pagamento.Domain.Aggregates.Pagamento(pedidoId, valor, FormaPagamento.QrCodeMercadoPago);
        }

        private async Task When_CadastrarPagamento()
        {
            var pagamentoRepository = Substitute.For<IPagamentoRepository>();
            var mercadoPagoMockadoService = Substitute.For<IMercadoPagoMockadoService>();
            var logger = Substitute.For<ILogger<PagamentoController>>();
            var pedidoGateway = Substitute.For<IPedidoGateway>();
            var rabbitMqService = Substitute.For<IRabbitMqService>();
            var rabbitMqOptions = Substitute.For<IOptions<RabbitOptions>>();

            pagamentoRepository.Cadastrar(_pagamento).ReturnsForAnyArgs(_pagamento);
            mercadoPagoMockadoService.GerarPagamentoEQrCode().Returns("qrcodedata");

            var pagamentoController = new PagamentoController(
                pagamentoRepository,
                new PagamentoPresenter(),
                mercadoPagoMockadoService,
                logger,
                pedidoGateway,
                rabbitMqService,
                rabbitMqOptions);

            _novoPagamentoDto = await pagamentoController.Cadastrar(_pagamento.PedidoId, _pagamento.FormaPagamento, _pagamento.Valor);
        }

        private void Then_PagamentoCriadoNaoDeveSerNulo()
        {
            Assert.NotNull(_novoPagamentoDto);
        }

        private void Then_TodosAsPropriedadesDevemSerIguais()
        {
            Assert.Equal(_pagamento.PedidoId, _novoPagamentoDto.PedidoId);
            Assert.Equal(_pagamento.Id, _novoPagamentoDto.Id);
            Assert.Equal(_pagamento.Valor, _novoPagamentoDto.Valor);
            Assert.Equal(_pagamento.StatusPagamento.ToString(), _novoPagamentoDto.StatusPagamento);
        }
    }
}
