//using Amazon.DynamoDBv2;
//using Amazon.DynamoDBv2.DataModel;
//using NSubstitute;
//using TechLanches.Pagamento.Adapter.DynamoDB.Models;
//using TechLanches.Pagamento.Adapter.DynamoDB.Repositories;
//using TechLanches.Pagamento.Domain.Aggregates;
//using TechLanches.Pagamento.Domain.Enums;

//namespace TechLanches.Pagamento.UnitTests.UnitTests.Adapter.Repositories
//{
//    public class PagamentoRepositoryTests
//    {
//        [Fact]
//        public async Task Atualizar_DeveAtualizarPagamentoCorretamente()
//        {
//            // Arrange
//            var pagamento = new Pagamento.Domain.Aggregates.Pagamento("1", 1, 100.0m, StatusPagamento.Aprovado);
//            var pagamentoDynamoModel = new PagamentoDbModel { Id = "1", PedidoId = 1, Valor = 100.0m, StatusPagamento = 1, FormaPagamento = 1 };
//            var dynamoDbClient = Substitute.For<IAmazonDynamoDB>();
//            var context = new DynamoDBContext(dynamoDbClient);
//            var repository = new PagamentoRepository(dynamoDbClient);


//            // Act
//            await repository.Atualizar(pagamento);

//            // Assert
//            await context.Received(1).SaveAsync(Arg.Any<PagamentoDbModel>());
//        }

//        [Fact]
//        public async Task BuscarPagamentoPorId_DeveRetornarPagamentoCorreto()
//        {
//            // Arrange
//            var pagamentoId = "1";
//            var pagamentoDynamoModel = new PagamentoDbModel { Id = pagamentoId, PedidoId = 1, Valor = 100.0m, StatusPagamento = 1, FormaPagamento = 1 };
//            var dynamoDbClient = Substitute.For<IAmazonDynamoDB>();
//            var context = new DynamoDBContext(dynamoDbClient);
//            context.LoadAsync<PagamentoDbModel>(pagamentoId).Returns(pagamentoDynamoModel);
//            var repository = new PagamentoRepository(dynamoDbClient);

//            // Act
//            var resultado = await repository.BuscarPagamentoPorId(pagamentoId);

//            // Assert
//            Assert.NotNull(resultado);
//            Assert.Equal(pagamentoId, resultado.Id);
//            Assert.Equal(1, resultado.PedidoId);
//            Assert.Equal(100.0m, resultado.Valor);
//            Assert.Equal(StatusPagamento.Aprovado, resultado.StatusPagamento);
//            Assert.Equal(FormaPagamento.QrCodeMercadoPago, resultado.FormaPagamento);
//        }

//        // Testes semelhantes para BuscarPagamentoPorPedidoId e Cadastrar
//    }
//}
