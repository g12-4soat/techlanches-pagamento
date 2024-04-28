namespace TechLanches.Pagamento.Domain.Enums.Pedido
{
    public enum StatusPedido
    {
        PedidoCriado = 1,
        PedidoRecebido,
        PedidoCancelado,
        PedidoCanceladoPorPagamentoRecusado,
        PedidoEmPreparacao,
        PedidoPronto,
        PedidoFinalizado,
        PedidoRetirado,
        PedidoDescartado
    }

}