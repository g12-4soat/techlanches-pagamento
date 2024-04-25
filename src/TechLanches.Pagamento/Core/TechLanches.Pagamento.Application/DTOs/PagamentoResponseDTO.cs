namespace TechLanches.Pagamento.Application.DTOs
{
    public class PagamentoResponseDTO
    {
        /// <summary>
        /// Id Pedido
        /// </summary>
        /// <example>1</example>
        public string Id { get; set; }
        public int PedidoId { get; set; }

        /// <summary>
        /// Valor Pagamento
        /// </summary>
        /// <example>15.50</example>
        public decimal Valor { get; set; }

        /// <summary>
        /// Código de pagamento
        /// </summary>
        /// <example>qrcodedata</example>
        public string QRCodeData { get; set; }

        /// <summary>
        /// Status Pagamento
        /// </summary>
        /// <example>Aprovado</example>
        public string StatusPagamento { get; set; }
    }
}
