using Microsoft.Extensions.Options;
using System.Drawing;
using System.Transactions;
using TechLanches.Pagamento.Adapter.ACL.QrCode.DTOs;
using TechLanches.Pagamento.Adapter.ACL.QrCode.Provedores.MercadoPago;
using TechLanches.Pagamento.Adapter.RabbitMq;
using TechLanches.Pagamento.Adapter.RabbitMq.Messaging;
using TechLanches.Pagamento.Adapter.RabbitMq.Options;
using TechLanches.Pagamento.Application.DTOs;
using TechLanches.Pagamento.Application.Gateways;
using TechLanches.Pagamento.Application.Gateways.Interfaces;
using TechLanches.Pagamento.Application.Ports.Repositories;
using TechLanches.Pagamento.Application.Presenters.Interfaces;
using TechLanches.Pagamento.Application.UseCases.Pagamentos;
using TechLanches.Pagamento.Domain.Enums;

namespace TechLanches.Pagamento.Application.Controllers
{
    public class PagamentoController : IPagamentoController
    {
        private readonly IPagamentoPresenter _pagamentoPresenter;
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly IMercadoPagoMockadoService _mercadoPagoMockadoService;
        private readonly IPagamentoGateway pagamentoGateway;
        private readonly IRabbitMqService _rabbitMqService;
        private readonly RabbitOptions _rabbitOptions;
        public PagamentoController(
            IPagamentoRepository pagamentoRepository,
            IPagamentoPresenter pagamentoPresenter,
            IMercadoPagoMockadoService mercadoPagoMockadoService,
            IRabbitMqService rabbitMqService,
            IOptions<RabbitOptions> rabbitOptions)
        {
            _pagamentoRepository = pagamentoRepository;
            _pagamentoPresenter = pagamentoPresenter;
            _mercadoPagoMockadoService = mercadoPagoMockadoService;
            pagamentoGateway = new PagamentoGateway(_pagamentoRepository, _mercadoPagoMockadoService);
            _rabbitMqService = rabbitMqService;
            _rabbitOptions = rabbitOptions.Value;
        }

        public async Task<PagamentoResponseDTO> BuscarPagamentoPorPedidoId(int pedidoId)
        {
            var pagamento = await pagamentoGateway.BuscarPagamentoPorPedidoId(pedidoId);

            return _pagamentoPresenter.ParaDto(pagamento);
        }

        public async Task<PagamentoResponseDTO> Cadastrar(int pedidoId, FormaPagamento formaPagamento, decimal valor)
        {
            var qrCode = await pagamentoGateway.GerarPagamentoEQrCode();
            var pagamento = await PagamentoUseCase.Cadastrar(pedidoId, formaPagamento, valor, pagamentoGateway);

            var pagamentoDTO = _pagamentoPresenter.ParaDto(pagamento);
            pagamentoDTO.QRCodeData = qrCode;

            return pagamentoDTO;
        }

        public async Task<PagamentoResponseACLDTO> ConsultarPagamentoMockado(string pedidoComercial)
        {
            return await pagamentoGateway.ConsultarPagamento(pedidoComercial);
        }

        public async Task<string> GerarQrCode()
        {
            return await pagamentoGateway.GerarPagamentoEQrCode();
        }

        public async Task ProcessarMensagem(PedidoCriadoMessage message)
        {
            await PagamentoUseCase.Cadastrar(message.Id, FormaPagamento.QrCodeMercadoPago, message.Valor, pagamentoGateway);
        }

        public async Task<bool> RealizarPagamento(int pedidoId, StatusPagamentoEnum statusPagamento)
        {
            var pagamento = await PagamentoUseCase.RealizarPagamento(pedidoId, statusPagamento, pagamentoGateway);
            await pagamentoGateway.Atualizar(pagamento);

            PedidoStatusMessage pedidoStatusMessage;

            var pagamentoFoiAprovado = pagamento.StatusPagamento == StatusPagamento.Aprovado;

            //if (pagamentoFoiAprovado)
            //    pedidoStatusMessage = new PedidoStatusMessage(pagamento.PedidoId, Domain.Enums.Pedido.StatusPedido.PedidoRecebido);
            //else
            //    pedidoStatusMessage = new PedidoStatusMessage(pagamento.PedidoId, Domain.Enums.Pedido.StatusPedido.PedidoCanceladoPorPagamentoRecusado);

            //_rabbitMqService.Publicar(pedidoStatusMessage, _rabbitOptions.QueueOrderStatus);

            return pagamentoFoiAprovado;
        }
    }
}
