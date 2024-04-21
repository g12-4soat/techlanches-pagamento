using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using TechLanches.Pagamento.Adapter.DynamoDB.Models;
using TechLanches.Pagamento.Application.Ports.Repositories;
using TechLanches.Pagamento.Domain.Enums;

namespace TechLanches.Pagamento.Adapter.DynamoDB.Repositories
{
    public class PagamentoRepository : IPagamentoRepository
    {
        private readonly DynamoDBContext _context;

        public PagamentoRepository(IAmazonDynamoDB dynamoDbClient)
        {
            _context = new DynamoDBContext(dynamoDbClient);
        }
        public async Task<Domain.Aggregates.Pagamento> Atualizar(Domain.Aggregates.Pagamento pagamento)
        {
            var pagamentoDynamoModel = new PagamentoDbModel(pagamento.PedidoId, pagamento.Valor, (int)pagamento.StatusPagamento, (int)pagamento.FormaPagamento);
            await _context.SaveAsync(pagamentoDynamoModel);

            return pagamento;
        }

        public async Task<Domain.Aggregates.Pagamento> BuscarPagamentoPorId(int pagamentoId)
        {
            var pagamentoDynamoModel = await _context.LoadAsync<PagamentoDbModel>(pagamentoId);
            var pagamento = new Domain.Aggregates.Pagamento(pagamentoDynamoModel.Id, pagamentoDynamoModel.PedidoId, pagamentoDynamoModel.Valor, (StatusPagamento)pagamentoDynamoModel.StatusPagamento);
            return pagamento;
        }

        public async Task<Domain.Aggregates.Pagamento> BuscarPagamentoPorPedidoId(int pedidoId)
        {
            var query = _context.QueryAsync<PagamentoDbModel>(pedidoId, new DynamoDBOperationConfig
            {
                IndexName = "pedidoIdIndex"
            });
            var pagamentoDynamoModel = (await query.GetNextSetAsync()).FirstOrDefault();
            var pagamento = new Domain.Aggregates.Pagamento(pagamentoDynamoModel.Id, pagamentoDynamoModel.PedidoId, pagamentoDynamoModel.Valor, (StatusPagamento)pagamentoDynamoModel.StatusPagamento);
            
            return pagamento;
        }

        public async Task<Domain.Aggregates.Pagamento> Cadastrar(Domain.Aggregates.Pagamento pagamento)
        {
            var pagamentoDynamoModel = new PagamentoDbModel(pagamento.PedidoId, pagamento.Valor, (int)pagamento.StatusPagamento, (int)pagamento.FormaPagamento);
            await _context.SaveAsync(pagamentoDynamoModel);

            return pagamento;
        }
    }
}
