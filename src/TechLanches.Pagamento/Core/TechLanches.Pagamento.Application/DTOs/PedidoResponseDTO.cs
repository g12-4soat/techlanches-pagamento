using TechLanches.Pagamento.Domain.Enums.Pedido;

namespace TechLanches.Pagamento.Application.DTOs
{
    public class PedidoResponseDTO
    {
        /// <summary>
        /// Id do pedido
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }


        /// <summary>
        /// Nome do Status do pedido 
        /// </summary>
        /// <example>PedidoCriado</example>
        public string NomeStatusPedido { get; set; }

        /// <summary>
        /// Status do pedido
        /// </summary>
        /// <example>PedidoCriado</example>
        public StatusPedido StatusPedido { get; set; }

        /// <summary>
        /// Valor total do pedido
        /// </summary>
        /// <example>12</example>
        public decimal Valor { get; set; }

    }
}
