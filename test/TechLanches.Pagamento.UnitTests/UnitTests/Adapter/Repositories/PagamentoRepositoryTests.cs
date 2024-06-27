using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using NSubstitute;
using TechLanches.Pagamento.Adapter.DynamoDB.Models;
using TechLanches.Pagamento.Adapter.DynamoDB.Repositories;
using TechLanches.Pagamento.Domain.Aggregates;
using TechLanches.Pagamento.Domain.Enums;

namespace TechLanches.Pagamento.UnitTests.UnitTests.Adapter.Repositories
{
    public class PagamentoRepositoryTests

    {
        [Fact]
        public async Task BuscarPagamentoPorId_DeveRetornarPagamentoCorreto()
        {
            // Arrange
            var pagamentoId = "1";
            var pagamentoDynamoModel = new PagamentoDbModel { Id = pagamentoId, PedidoId = 1, Valor = 100.0m, StatusPagamento = 1, FormaPagamento = 1 };
            var context = Substitute.For<IDynamoDBContext>();
            context.LoadAsync<PagamentoDbModel>(pagamentoId).Returns(pagamentoDynamoModel);
            var repository = new PagamentoRepository(context);

            // Act
            var resultado = await repository.BuscarPagamentoPorId(pagamentoId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(pagamentoId, resultado.Id);
            Assert.Equal(1, resultado.PedidoId);
            Assert.Equal(100.0m, resultado.Valor);
        }

        [Fact]
        public async Task BuscarPagamentoPorId_DeveRetornarNull()
        {
            // Arrange
            var pagamentoId = "pagamentoId";
            PagamentoDbModel nullModel = null;

            var context = Substitute.For<IDynamoDBContext>();
            context.LoadAsync<PagamentoDbModel>(pagamentoId).Returns(nullModel);

            var repository = new PagamentoRepository(context);

            // Act
            var pagamento = await repository.BuscarPagamentoPorId(pagamentoId);

            // Assert
            Assert.Null(pagamento);
        }

        [Fact]
        public async Task Cadastrar_DeveCadastrarPagamentoCorretamente()
        {
            // Arrange
            var pagamento = new Pagamento.Domain.Aggregates.Pagamento("1", 1, 100.0m, StatusPagamento.Aguardando);

            var context = Substitute.For<IDynamoDBContext>();

            var repository = new PagamentoRepository(context);

            // Act
            var novoPagamento = await repository.Cadastrar(pagamento);

            // Assert
            Assert.Equal(1, novoPagamento.PedidoId);
            Assert.Equal(100.00m, novoPagamento.Valor);
            Assert.Equal(StatusPagamento.Aguardando, novoPagamento.StatusPagamento);
        }

        [Fact]
        public async Task BuscarPagamentoPorPedidoId_DeveRetornarPagamento_CasoExista()
        {
            // Arrange
            var pedidoId = 123;
            var pagamentoDynamoModel = new PagamentoDbModel
            {
                Id = "pagamentoId",
                PedidoId = pedidoId,
                Valor = 100.00m,
                StatusPagamento = (int)StatusPagamento.Aprovado
            };

            var query = Substitute.For<AsyncSearch<PagamentoDbModel>>();
            query.GetNextSetAsync().Returns(Task.FromResult(new List<PagamentoDbModel> { pagamentoDynamoModel }));

            var context = Substitute.For<IDynamoDBContext>();
            context.QueryAsync<PagamentoDbModel>(pedidoId, Arg.Any<DynamoDBOperationConfig>()).Returns(query);

            var repository = new PagamentoRepository(context);

            // Act
            var pagamento = await repository.BuscarPagamentoPorPedidoId(pedidoId);

            // Assert
            Assert.NotNull(pagamento);
            Assert.Equal("pagamentoId", pagamento.Id);
            Assert.Equal(pedidoId, pagamento.PedidoId);
            Assert.Equal(100.00m, pagamento.Valor);
            Assert.Equal(StatusPagamento.Aprovado, pagamento.StatusPagamento);
        }

        [Fact]
        public async Task BuscarPagamentoPorPedidoId_DeveRetornarNull_CasoNaoExista()
        {
            // Arrange
            var pedidoId = 123;
           

            var query = Substitute.For<AsyncSearch<PagamentoDbModel>>();
            query.GetNextSetAsync().Returns(Task.FromResult(new List<PagamentoDbModel> {}));

            var context = Substitute.For<IDynamoDBContext>();
            context.QueryAsync<PagamentoDbModel>(pedidoId, Arg.Any<DynamoDBOperationConfig>()).Returns(query);

            var repository = new PagamentoRepository(context);

            // Act
            var pagamento = await repository.BuscarPagamentoPorPedidoId(pedidoId);

            // Assert
            Assert.Null(pagamento);
        }
    }
}
