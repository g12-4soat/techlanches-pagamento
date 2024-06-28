using Microsoft.Extensions.Logging;
using TechLanches.Pagamento.Adapter.ACL.QrCode.DTOs;
using TechLanches.Pagamento.Adapter.ACL.QrCode.Provedores.MercadoPago;
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
        private readonly IPagamentoGateway _pagamentoGateway;
        private readonly IPedidoGateway _pedidoGateway;
        private readonly ILogger<PagamentoController> _logger;

        public PagamentoController(
            IPagamentoRepository pagamentoRepository,
            IPagamentoPresenter pagamentoPresenter,
            IMercadoPagoMockadoService mercadoPagoMockadoService,
            ILogger<PagamentoController> logger,
            IPedidoGateway pedidoGateway)
        {
            _pagamentoRepository = pagamentoRepository;
            _pagamentoPresenter = pagamentoPresenter;
            _mercadoPagoMockadoService = mercadoPagoMockadoService;
            _pagamentoGateway = new PagamentoGateway(_pagamentoRepository, _mercadoPagoMockadoService);
            _logger = logger;
            _pedidoGateway = pedidoGateway;
        }

        public async Task<PagamentoResponseDTO> BuscarPagamentoPorPedidoId(int pedidoId)
        {
            var pagamento = await _pagamentoGateway.BuscarPagamentoPorPedidoId(pedidoId);

            return _pagamentoPresenter.ParaDto(pagamento);
        }

        public async Task<PagamentoResponseDTO> Cadastrar(int pedidoId, FormaPagamento formaPagamento, decimal valor)
        {
            var qrCode = await _pagamentoGateway.GerarPagamentoEQrCode();
            var pagamento = await PagamentoUseCase.Cadastrar(pedidoId, formaPagamento, valor, _pagamentoGateway);

            var pagamentoDTO = _pagamentoPresenter.ParaDto(pagamento);
            pagamentoDTO.QRCodeData = qrCode;

            return pagamentoDTO;
        }

        public async Task<PagamentoResponseACLDTO> ConsultarPagamentoMockado(string pedidoComercial)
        {
            return await _pagamentoGateway.ConsultarPagamento(pedidoComercial);
        }

        public async Task<bool> InativarPagamentos(string cpf)
        {
            bool sucesso = true;

            var pedidos = await _pedidoGateway.BuscarPedidosPorCpf(cpf);

            if (pedidos is null || pedidos.Count == 0)
            {
                _logger.LogInformation("Não existem pagamentos a serem inativados");
                return sucesso;
            }

            var pedidosId = pedidos.Select(x => x.Id).ToList();
            var pagamentos = await _pagamentoRepository.BuscarPagamentosPorPedidosId(pedidosId);

            foreach (var pagamento in pagamentos)
            {
                Domain.Aggregates.Pagamento retorno;

                pagamento.Inativar();

                try
                {
                    retorno = await _pagamentoGateway.Atualizar(pagamento);

                    if (retorno.Ativo)
                    {
                        _logger.LogError("Ocorreu um problema ao tentar inativar o pagamento {pagamentoId}", pagamento.Id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro não identificado ao tentar inativar o pagamento {pagamentoId}", pagamento.Id);

                    throw;
                }

                sucesso = sucesso && !retorno.Ativo;
            }

            return sucesso;
        }

        public async Task<bool> RealizarPagamento(int pedidoId, StatusPagamentoEnum statusPagamento)
        {
            var pagamento = await PagamentoUseCase.RealizarPagamento(pedidoId, statusPagamento, _pagamentoGateway);
            await _pagamentoGateway.Atualizar(pagamento);

            return pagamento.StatusPagamento == StatusPagamento.Aprovado;
        }
    }
}