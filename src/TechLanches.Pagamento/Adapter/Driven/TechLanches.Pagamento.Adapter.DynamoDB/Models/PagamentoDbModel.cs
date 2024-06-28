using Amazon.DynamoDBv2.DataModel;

namespace TechLanches.Pagamento.Adapter.DynamoDB.Models
{
    [DynamoDBTable("pagamentos")]
    public class PagamentoDbModel
    {
        public PagamentoDbModel(int pedidoId, decimal valor, int statusPagamento, int formaPagamento, bool ativo)
        {
            Id = Guid.NewGuid().ToString();
            PedidoId = pedidoId;
            Valor = valor;
            StatusPagamento = statusPagamento;
            FormaPagamento = formaPagamento;
            Ativo = ativo;
        }

        public PagamentoDbModel()
        {
            
        }

        [DynamoDBHashKey]
        public string Id { get; set; }
        public int PedidoId { get; set; }
        public decimal Valor { get; set; }
        public int StatusPagamento { get; set; }
        public int FormaPagamento { get; set; }
        public bool Ativo { get; set; }
    }
}
