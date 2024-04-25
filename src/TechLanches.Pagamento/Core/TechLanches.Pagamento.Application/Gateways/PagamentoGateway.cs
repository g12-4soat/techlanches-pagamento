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
        private readonly ApplicationOptions _applicationOptions;

        public PagamentoGateway(
            IPagamentoRepository pagamentoRepository,
            IMercadoPagoMockadoService mercadoPagoMockadoService,
            ApplicationOptions applicationOptions)
        {
            _pagamentoRepository = pagamentoRepository;
            _mercadoPagoMockadoService = mercadoPagoMockadoService;
            _applicationOptions = applicationOptions;
        }

        public Task<Domain.Aggregates.Pagamento> Atualizar(Domain.Aggregates.Pagamento pagamento)
        {
            return _pagamentoRepository.Atualizar(pagamento);
        }

        public Task<Domain.Aggregates.Pagamento> BuscarPagamentoPorPedidoId(int pedidoId)
        {
            return _pagamentoRepository.BuscarPagamentoPorPedidoId(pedidoId);
        }

        public Task<Domain.Aggregates.Pagamento> Cadastrar(Domain.Aggregates.Pagamento pagamento)
        {
            return _pagamentoRepository.Cadastrar(pagamento);
        }

        public async Task<PagamentoResponseACLDTO> ConsultarPagamento(string pedidoComercial)
        {
            return await _mercadoPagoMockadoService.ConsultarPagamento(pedidoComercial);
        }

        public async Task<string> GerarPagamentoEQrCode()
        {
            return await _mercadoPagoMockadoService.GerarPagamentoEQrCode();
        }
    }
}