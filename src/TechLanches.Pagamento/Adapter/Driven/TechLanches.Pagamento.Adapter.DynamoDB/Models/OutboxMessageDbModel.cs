using Amazon.DynamoDBv2.DataModel;

namespace TechLanches.Pagamento.Adapter.DynamoDB.Models
{
    [DynamoDBTable("outboxMessage")]
    public class OutboxMessageDbModel
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
        public string PagamentoId { get; set; }
        public string Processado { get; set; }
    }
}