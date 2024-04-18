using System.Text.Json;
using TechLanches.Pagamento.Adapter.ACL.QrCode.DTOs;
using TechLanches.Pagamento.Adapter.ACL.QrCode.Provedores.MercadoPago;
using TechLanches.Pagamento.Application.Gateways.Interfaces;
using TechLanches.Pagamento.Application.Options;
using TechLanches.Pagamento.Application.Ports.Repositories;

namespace TechLanches.Pagamento.Application.Gateways
{
    public class PagamentoGateway : IPagamentoGateway
    {
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly IMercadoPagoMockadoService _mercadoPagoMockadoService;
        private readonly IMercadoPagoService _mercadoPagoService;
        private readonly ApplicationOptions _applicationOptions;
        private readonly bool _mockado;

        public PagamentoGateway(
            IPagamentoRepository pagamentoRepository,
            IMercadoPagoMockadoService mercadoPagoMockadoService,
            IMercadoPagoService mercadoPagoService,
            ApplicationOptions applicationOptions,
            bool mockado)
        {
            _pagamentoRepository = pagamentoRepository;
            _mercadoPagoMockadoService = mercadoPagoMockadoService;
            _mercadoPagoService = mercadoPagoService;
            _applicationOptions = applicationOptions;
            _mockado = mockado;
        }

        public Task<Domain.Aggregates.Pagamento> BuscarPagamentoPorPedidoId(int pedidoId)
        {
            return _pagamentoRepository.BuscarPagamentoPorPedidoId(pedidoId);
        }

        public Task<Domain.Aggregates.Pagamento> Cadastrar(Domain.Aggregates.Pagamento pagamento)
        {
            return _pagamentoRepository.Cadastrar(pagamento);
        }

        public async Task<PagamentoResponseACLDTO> ConsultarPagamentoMercadoPago(string pedidoComercial)
        {
            if (_mockado)
            {
                return await _mercadoPagoMockadoService.ConsultarPagamento(pedidoComercial);
            }

            return await _mercadoPagoService.ConsultarPagamento(pedidoComercial);
        }

        public async Task<string> GerarPagamentoEQrCodeMercadoPago(PedidoACLDTO pedidoMercadoPago)
        {
            var pedido = JsonSerializer.Serialize(pedidoMercadoPago);

            if (_mockado)
            {
                return await _mercadoPagoMockadoService.GerarPagamentoEQrCode(pedido, _applicationOptions.UserId, _applicationOptions.PosId);
            }

            return await _mercadoPagoService.GerarPagamentoEQrCode(pedido, _applicationOptions.UserId, _applicationOptions.PosId);
        }

        public async Task CommitAsync()
        {
            await _pagamentoRepository.UnitOfWork.CommitAsync();
        }
    }
}