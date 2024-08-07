﻿using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using TechLanches.Pagamento.Adapter.ACL.QrCode.DTOs;
using TechLanches.Pagamento.Adapter.ACL.QrCode.Provedores.MercadoPago;
using TechLanches.Pagamento.Adapter.RabbitMq;
using TechLanches.Pagamento.Adapter.RabbitMq.Messaging;
using TechLanches.Pagamento.Adapter.RabbitMq.Options;
using TechLanches.Pagamento.Application.Controllers;
using TechLanches.Pagamento.Application.DTOs;
using TechLanches.Pagamento.Application.Gateways.Interfaces;
using TechLanches.Pagamento.Application.Ports.Repositories;
using TechLanches.Pagamento.Application.Presenters.Interfaces;
using TechLanches.Pagamento.Domain.Enums;

namespace TechLanches.Pagamento.UnitTests.UnitTests.Application.Controllers
{
    public class PagamentoControllerTests
    {
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly IPagamentoPresenter _pagamentoPresenter;
        private readonly IMercadoPagoMockadoService _mercadoPagoMockadoService;
        private readonly PagamentoController _pagamentoController;
        private readonly ILogger<PagamentoController> _logger;
        private readonly IPedidoGateway _pedidoGateway;
        private readonly IRabbitMqService _rabbitMqService;
        private readonly IOptions<RabbitOptions> _rabbitMqOptions;

        public PagamentoControllerTests()
        {
            _pagamentoRepository = Substitute.For<IPagamentoRepository>();
            _pagamentoPresenter = Substitute.For<IPagamentoPresenter>();
            _mercadoPagoMockadoService = Substitute.For<IMercadoPagoMockadoService>();
            _rabbitMqService = Substitute.For<IRabbitMqService>();
            _rabbitMqOptions = Options.Create(new RabbitOptions { QueueOrderStatus = "teste"});

            _mercadoPagoMockadoService = Substitute.For<IMercadoPagoMockadoService>();
            _logger = Substitute.For<ILogger<PagamentoController>>();
            _pedidoGateway = Substitute.For<IPedidoGateway>();

            _pagamentoController = new PagamentoController(
                _pagamentoRepository,
                _pagamentoPresenter,
                _mercadoPagoMockadoService,
                _logger,
                _pedidoGateway,
                _rabbitMqService,
                _rabbitMqOptions
            );
        }

        [Fact]
        public async Task BuscarPagamentoPorPedidoId_DeveRetornarPagamentoDto()
        {   
            var pedidoId = 1;
            var valor = 100;

            var pagamento = new Pagamento.Domain.Aggregates.Pagamento(pedidoId, valor, FormaPagamento.QrCodeMercadoPago);
            _pagamentoRepository.BuscarPagamentoPorPedidoId(pedidoId).Returns(pagamento);
            var expectedDto = new PagamentoResponseDTO();

            _pagamentoPresenter.ParaDto(pagamento).Returns(expectedDto);

            var result = await _pagamentoController.BuscarPagamentoPorPedidoId(pedidoId);

            Assert.Equal(expectedDto, result);
        }

        [Fact]
        public async Task Cadastrar_DeveRetornarPagamentoDtoComQRCodeData()
        {
            // Arrange
            var pedidoId = 1;
            var valor = 100;
            var qrCodeData = "QRCodeData";
            var formaPagamento = FormaPagamento.QrCodeMercadoPago;


            var pagamento = new Pagamento.Domain.Aggregates.Pagamento(pedidoId, valor, formaPagamento);
            _mercadoPagoMockadoService.GerarPagamentoEQrCode().Returns(qrCodeData);
            _pagamentoRepository.Cadastrar(Arg.Any<Pagamento.Domain.Aggregates.Pagamento>()).Returns(pagamento);


            var expectedDto = new PagamentoResponseDTO();
            _pagamentoPresenter.ParaDto(pagamento).Returns(expectedDto);

            // Act
            var result = await _pagamentoController.Cadastrar(pedidoId, formaPagamento, valor);

            // Assert
            Assert.Equal(expectedDto, result);
            Assert.Equal(qrCodeData, result.QRCodeData);
        }

        [Fact]
        public async Task ConsultarPagamentoMockado_DeveRetornarPagamentoResponseACLDto()
        {
            // Arrange
            var pedidoId = "1";
            var expectedDto = new PagamentoResponseACLDTO();
            _mercadoPagoMockadoService.ConsultarPagamento(pedidoId).Returns(expectedDto);

            // Act
            var result = await _pagamentoController.ConsultarPagamentoMockado(pedidoId);

            // Assert
            Assert.Equal(expectedDto, result);
        }

        [Fact]
        public async Task RealizarPagamento_DeveAtualizarPagamentoEVerificarStatus()
        {
            // Arrange
            var pedidoId = 1;
            var valor = 100;
            var formaPagamento = FormaPagamento.QrCodeMercadoPago;
            var statusPagamento = StatusPagamentoEnum.Aprovado;

            var pagamento = new Pagamento.Domain.Aggregates.Pagamento(pedidoId, valor, formaPagamento);
            _pagamentoRepository.BuscarPagamentoPorPedidoId(pedidoId).Returns(pagamento);
            _pagamentoRepository.Atualizar(pagamento).Returns(pagamento);
            _pagamentoPresenter.ParaDto(pagamento).Returns(new PagamentoResponseDTO());
            _rabbitMqService.Publicar(Arg.Any<IBaseMessage>(), Arg.Any<string>());
            // Act
            var result = await _pagamentoController.RealizarPagamento(pedidoId, statusPagamento);

            // Assert
            Assert.True(result);
            await _pagamentoRepository.Received(1).Atualizar(pagamento);
        }
    }
}
