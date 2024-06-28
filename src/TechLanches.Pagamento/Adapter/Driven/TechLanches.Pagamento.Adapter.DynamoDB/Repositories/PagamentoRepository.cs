using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using TechLanches.Pagamento.Adapter.DynamoDB.Models;
using TechLanches.Pagamento.Application.Ports.Repositories;
using TechLanches.Pagamento.Domain.Enums;

namespace TechLanches.Pagamento.Adapter.DynamoDB.Repositories
{
    public class PagamentoRepository : IPagamentoRepository
    {
        private readonly IDynamoDBContext _context;
        private readonly IAmazonDynamoDB _dynamoDbClient;
        public PagamentoRepository(IAmazonDynamoDB dynamoDbClient)
        {
            _context = new DynamoDBContext(dynamoDbClient);
            _dynamoDbClient = dynamoDbClient;
        }

        public PagamentoRepository(IDynamoDBContext context)
        {
            _context = context;
        }

        public async Task<Domain.Aggregates.Pagamento> AtualizarDados(Domain.Aggregates.Pagamento pagamento)
        {
            var pagamentoDynamoModel = await _context.LoadAsync<PagamentoDbModel>(pagamento.Id);

            pagamentoDynamoModel.StatusPagamento = (int)pagamento.StatusPagamento;
            pagamentoDynamoModel.Ativo = pagamento.Ativo;

            await _context.SaveAsync(pagamentoDynamoModel);

            return pagamento;
        }

        public async Task<Domain.Aggregates.Pagamento> Atualizar(Domain.Aggregates.Pagamento pagamento)
        {
            // Carrega o modelo do DynamoDB baseado no Id do pagamento
            var pagamentoDynamoModel = await _context.LoadAsync<PagamentoDbModel>(pagamento.Id);

            // Verifica se o modelo foi encontrado
            if (pagamentoDynamoModel == null)
            {
                throw new Exception("Pagamento não encontrado.");
            }

            // Atualiza o status do pagamento no modelo do DynamoDB
            pagamentoDynamoModel.StatusPagamento = (int)pagamento.StatusPagamento;

            // Configuração da atualização
            var updatePagamentoRequest = new Update
            {
                TableName = "pagamentos",
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { S = pagamento.Id } }
                },
                UpdateExpression = "set StatusPagamento = :statusPagamento",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":statusPagamento", new AttributeValue { N = ((int)pagamento.StatusPagamento).ToString() } }
                }
            };

            // Configuração da transação
            var transactWriteItemsRequest = new TransactWriteItemsRequest
            {
                TransactItems = new List<TransactWriteItem>
                {
                    new TransactWriteItem
                    {
                        Update = updatePagamentoRequest
                    }
                }
            };

            try
            {
                // Executa a transação usando o cliente DynamoDB
                var response = await _dynamoDbClient.TransactWriteItemsAsync(transactWriteItemsRequest);

                // Verifica a resposta da transação
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return pagamento;
                }
                else
                {
                    throw new Exception("Transação falhou ao atualizar o pagamento.");
                }
            }
            catch (Exception ex)
            {
                // Lida com a exceção
                throw new Exception("Erro ao realizar transação para atualizar o pagamento.", ex);
            }
        }


        public async Task<Domain.Aggregates.Pagamento> BuscarPagamentoPorId(string pagamentoId)
        {
            var pagamentoDynamoModel = await _context.LoadAsync<PagamentoDbModel>(pagamentoId);

            if (pagamentoDynamoModel is null)
                return null;

            var pagamento = new Domain.Aggregates.Pagamento(
                pagamentoDynamoModel.Id,
                pagamentoDynamoModel.PedidoId,
                pagamentoDynamoModel.Valor,
                (StatusPagamento)pagamentoDynamoModel.StatusPagamento,
                pagamentoDynamoModel.Ativo);

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

            var pagamento = new Domain.Aggregates.Pagamento(
                pagamentoDynamoModel.Id,
                pagamentoDynamoModel.PedidoId,
                pagamentoDynamoModel.Valor,
                (StatusPagamento)pagamentoDynamoModel.StatusPagamento,
                pagamentoDynamoModel.Ativo);

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

        public async Task<List<Domain.Aggregates.Pagamento>> BuscarPagamentosPorPedidosId(List<int> pedidosId)
        {
            List<Domain.Aggregates.Pagamento> pagamentos = new();

            foreach (var pedidoId in pedidosId)
            {
                var pagamento = await BuscarPagamentoPorPedidoId(pedidoId);

                if (pagamento is not null)
                {
                    pagamentos.Add(pagamento);
                }
            }

            return pagamentos;
        }
    }
}
