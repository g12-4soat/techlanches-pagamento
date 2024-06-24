using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using TechLanches.Pagamento.Application.Ports.Repositories;
using TechLanches.Pagamento.Domain.Aggregates;
using TechLanches.Pagamento.Adapter.DynamoDB.Models;
using Amazon.DynamoDBv2.DocumentModel;

namespace TechLanches.Pagamento.Adapter.DynamoDB.Repositories
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly IDynamoDBContext _context;
        private readonly IAmazonDynamoDB _dynamoDbClient;

        public OutboxRepository(IAmazonDynamoDB dynamoDbClient)
        {
            _context = new DynamoDBContext(dynamoDbClient);
            _dynamoDbClient = dynamoDbClient;
        }

        public async Task<IEnumerable<Outbox>> ObterNaoProcessados()
        {
            var scanConditions = new List<ScanCondition>
            {
                new ScanCondition("Processado", ScanOperator.Equal, "0")
            };

            var search = _context.ScanAsync<OutboxMessageDbModel>(scanConditions);
            var outboxMessages = await search.GetNextSetAsync();

            var outboxList = outboxMessages.Take(10).Select(om => new Outbox
            {
                Id = om.Id,
                PagamentoId = om.PagamentoId,
                Processado = om.Processado
            }).ToList();

            return outboxList;
        }

        public async Task ProcessarMensagem(Outbox message)
        {
            var outboxDynamoModel = await _context.LoadAsync<OutboxMessageDbModel>(message.Id);

            outboxDynamoModel.Processado = message.Processado;
            await _context.SaveAsync(outboxDynamoModel);
        }
    }
}
