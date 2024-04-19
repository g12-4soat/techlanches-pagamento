using TechLanches.Pagamento.Domain.Enums;

namespace TechLanches.Pagamento.Application.DTOs
{
    public class PagamentoRequestDTO
    {
        /// <summary>
        /// Id Pedido
        /// </summary>
        /// <example>1</example>
        public int PedidoId { get; set; }

        /// <summary>
        /// Valor Pagamento
        /// </summary>
        /// <example>15.50</example>
        public decimal Valor { get; set; }
        public FormaPagamento FormaPagamento { get; set; } = Domain.Enums.FormaPagamento.QrCodeMercadoPago;
    }
}
