namespace TechLanches.Pagamento.Adapter.ACL.QrCode.DTOs
{
    public record PagamentoResponseACLDTO
    {
        public StatusPagamentoEnum StatusPagamento { get; set; }
        public int PedidoId { get; set; }
    }
}
