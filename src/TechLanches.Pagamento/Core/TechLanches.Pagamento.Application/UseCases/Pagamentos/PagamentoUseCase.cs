using TechLanches.Pagamento.Adapter.ACL.QrCode.DTOs;
using TechLanches.Pagamento.Application.Gateways.Interfaces;
using TechLanches.Pagamento.Core;
using TechLanches.Pagamento.Domain.Enums;

namespace TechLanches.Pagamento.Application.UseCases.Pagamentos
{
    public class PagamentoUseCase
    {
        public static async Task<Domain.Aggregates.Pagamento> Cadastrar(int pedidoId, FormaPagamento formaPagamento, decimal valor, IPagamentoGateway pagamentoGateway)
        {
            await VerificarPagamentoExistente(pedidoId, pagamentoGateway);

            Domain.Aggregates.Pagamento pagamento = new(pedidoId, valor, formaPagamento);
            return await pagamentoGateway.Cadastrar(pagamento);
        }

        public static async Task<Domain.Aggregates.Pagamento> RealizarPagamento(int pedidoId, StatusPagamentoEnum statusPagamento, IPagamentoGateway pagamentoGateway)
        {
            var pagamento = await VerificarPagamentoNaoExistente(pedidoId, pagamentoGateway);

            if (statusPagamento == StatusPagamentoEnum.Aprovado)
                pagamento.Aprovar();
            else
                pagamento.Reprovar();

            return pagamento;
        }

        private static async Task<Domain.Aggregates.Pagamento> VerificarPagamentoExistente(int id, IPagamentoGateway pagamentoGateway)
        {
            var pagamento = await pagamentoGateway.BuscarPagamentoPorPedidoId(id);

            if (pagamento is not null)
                throw new DomainException($"Pagamento já cadastrado para o pedido: {id} com status: {pagamento.StatusPagamento}.");

            return pagamento;
        }
        private static async Task<Domain.Aggregates.Pagamento> VerificarPagamentoNaoExistente(int id, IPagamentoGateway pagamentoGateway)
        {
            var pagamento = await pagamentoGateway.BuscarPagamentoPorPedidoId(id)
                ?? throw new DomainException($"É necessário realizar o checkout primeiro antes de realizar o pagamento.");

            if (pagamento.StatusPagamento == StatusPagamento.Aprovado)
                throw new DomainException($"Pagamento já efetuado para o pedido: {id}.");

            return pagamento;
        }
    }
}
