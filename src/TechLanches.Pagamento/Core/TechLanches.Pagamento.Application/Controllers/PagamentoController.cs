using Microsoft.Extensions.Options;
using TechLanches.Pagamento.Adapter.ACL.QrCode.DTOs;
using TechLanches.Pagamento.Adapter.ACL.QrCode.Provedores.MercadoPago;
using TechLanches.Pagamento.Application.DTOs;
using TechLanches.Pagamento.Application.Gateways;
using TechLanches.Pagamento.Application.Gateways.Interfaces;
using TechLanches.Pagamento.Application.Options;
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
        private readonly ApplicationOptions _applicationOptions;
        private readonly IPagamentoGateway pagamentoGateway;
        public PagamentoController(
            IPagamentoRepository pagamentoRepository,
            IPagamentoPresenter pagamentoPresenter,
            IMercadoPagoMockadoService mercadoPagoMockadoService,
            IOptions<ApplicationOptions> applicationOptions)
        {
            _pagamentoRepository = pagamentoRepository;
            _pagamentoPresenter = pagamentoPresenter;
            _mercadoPagoMockadoService = mercadoPagoMockadoService;
            _applicationOptions = applicationOptions.Value;
            pagamentoGateway = new PagamentoGateway(_pagamentoRepository, _mercadoPagoMockadoService, _applicationOptions);
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


        public async Task<bool> RealizarPagamento(int pedidoId, StatusPagamentoEnum statusPagamento)
        {
            var pagamento = await PagamentoUseCase.RealizarPagamento(pedidoId, statusPagamento, pagamentoGateway);
            await pagamentoGateway.Atualizar(pagamento);

            return pagamento.StatusPagamento == StatusPagamento.Aprovado;
        }
    }
}