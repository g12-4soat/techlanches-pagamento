using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using TechLanches.Pagamento.Adapter.DynamoDB.Models;
using TechLanches.Pagamento.Application.Ports.Repositories;
using TechLanches.Pagamento.Domain.Enums;

namespace TechLanches.Pagamento.Adapter.DynamoDB.Repositories
{
    public class PagamentoRepository : IPagamentoRepository
    {
        private readonly IDynamoDBContext _context;

        public PagamentoRepository(IAmazonDynamoDB dynamoDbClient)
        {
            _context = new DynamoDBContext(dynamoDbClient);
        }

        public PagamentoRepository(IDynamoDBContext context)
        {
            _context = context;
        }

        public async Task<Domain.Aggregates.Pagamento> Atualizar(Domain.Aggregates.Pagamento pagamento)
        {
            var pagamentoDynamoModel = await _context.LoadAsync<PagamentoDbModel>(pagamento.Id);

            pagamentoDynamoModel.StatusPagamento = (int)pagamento.StatusPagamento;
            pagamentoDynamoModel.Ativo = pagamento.Ativo;

            await _context.SaveAsync(pagamentoDynamoModel);

            return pagamento;
        }

        public async Task<Domain.Aggregates.Pagamento> BuscarPagamentoPorId(string pagamentoId)
        {
            var pagamentoDynamoModel = await _context.LoadAsync<PagamentoDbModel>(pagamentoId);

            if (pagamentoDynamoModel is null)
                return null;

            var pagamento = new Domain.Aggregates.Pagamento(pagamentoDynamoModel.Id, pagamentoDynamoModel.PedidoId, pagamentoDynamoModel.Valor, (StatusPagamento)pagamentoDynamoModel.StatusPagamento);
            return pagamento;
        }

        public async Task<Domain.Aggregates.Pagamento> BuscarPagamentoPorPedidoId(int pedidoId)
        {
            AsyncSearch<PagamentoDbModel> query = _context.QueryAsync<PagamentoDbModel>(pedidoId, new DynamoDBOperationConfig
            {
                IndexName = "pedidoIdIndex"
            });
            PagamentoDbModel pagamentoDynamoModel = (await query.GetNextSetAsync()).Where(x => x.Ativo).FirstOrDefault();

            if (pagamentoDynamoModel is null)
                return null;

            var pagamento = new Domain.Aggregates.Pagamento(pagamentoDynamoModel.Id, pagamentoDynamoModel.PedidoId, pagamentoDynamoModel.Valor, (StatusPagamento)pagamentoDynamoModel.StatusPagamento);

            return pagamento;
        }

        public async Task<Domain.Aggregates.Pagamento> Cadastrar(Domain.Aggregates.Pagamento pagamento)
        {
            var pagamentoDynamoModel = new PagamentoDbModel(
                pagamento.PedidoId,
                pagamento.Valor,
                (int)pagamento.StatusPagamento,
                (int)pagamento.FormaPagamento,
                pagamento.Ativo);
            await _context.SaveAsync(pagamentoDynamoModel);

            return pagamento;
        }
    }
}
