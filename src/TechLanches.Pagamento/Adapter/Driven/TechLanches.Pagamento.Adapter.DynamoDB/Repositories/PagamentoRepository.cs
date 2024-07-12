using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TechLanches.Pagamento.Adapter.DynamoDB.Models;
using TechLanches.Pagamento.Adapter.RabbitMq;
using TechLanches.Pagamento.Application.Ports.Repositories;
using TechLanches.Pagamento.Domain.Enums;

namespace TechLanches.Pagamento.Adapter.DynamoDB.Repositories
{
    public class PagamentoRepository : IPagamentoRepository
    {
        private readonly IDynamoDBContext _context;
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly ILogger<PagamentoRepository> _logger;

        public PagamentoRepository(IAmazonDynamoDB dynamoDbClient, ILogger<PagamentoRepository> logger)
        {
            _context = new DynamoDBContext(dynamoDbClient);
            _dynamoDbClient = dynamoDbClient;
            _logger = logger;
        }

        public PagamentoRepository(IDynamoDBContext context, ILogger<PagamentoRepository> logger)
        {
            _context = context;
            _logger = logger;
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
            var pagamentoDynamoModel = await _context.LoadAsync<PagamentoDbModel>(pagamento.Id);

            if (pagamentoDynamoModel == null)
            {
                throw new Exception("Pagamento não encontrado.");
            }

            pagamentoDynamoModel.StatusPagamento = (int)pagamento.StatusPagamento;

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

            var outboxMessage = new OutboxMessageDbModel
            {
                Id = Guid.NewGuid().ToString(),
                PagamentoId = pagamento.Id,
                Processado = "0",
            };

            // Configuração da inserção na tabela Outbox
            var putOutboxRequest = new Put
            {
                TableName = "outboxMessage",
                Item = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { S = outboxMessage.Id } },
                    { "PagamentoId", new AttributeValue { S = outboxMessage.PagamentoId } },
                    { "Processado", new AttributeValue { S = outboxMessage.Processado.ToString()} },
                }
            };

            var transactWriteItemsRequest = new TransactWriteItemsRequest
            {
                TransactItems = new List<TransactWriteItem>
                {
                    new TransactWriteItem { Update = updatePagamentoRequest },
                    new TransactWriteItem { Put = putOutboxRequest }
                }
            };

            var response = await _dynamoDbClient.TransactWriteItemsAsync(transactWriteItemsRequest);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return pagamento;
            }
            else
            {
                throw new Exception("Transação falhou ao atualizar o pagamento.");
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

            var pagamentos = await query.GetNextSetAsync();

            _logger.LogInformation("pagamentos encontrados ", JsonSerializer.Serialize(pagamentos));

            PagamentoDbModel pagamentoDynamoModel = pagamentos?.FirstOrDefault(x => x.Ativo);

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
