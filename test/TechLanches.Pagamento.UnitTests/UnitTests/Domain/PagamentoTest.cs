using TechLanches.Pagamento.Core;
using TechLanches.Pagamento.Domain.Enums;

namespace TechLanches.Pagamento.UnitTests.UnitTests.Domain
{
    [Trait("Domain", "Pagamento")]
    public class PagamentoTest
    {
        [Fact(DisplayName = "Criar pagamento com sucesso")]
        public void CriarPagamento_DeveRetornarSucesso()
        {
            //Arrange    
            var pedidoId = 1;
            var valor = 100;

            //Act 
            var pagamento = new Pagamento.Domain.Aggregates.Pagamento(pedidoId, valor, FormaPagamento.QrCodeMercadoPago);

            //Assert
            Assert.NotNull(pagamento);
            Assert.Equal(pedidoId, pagamento.PedidoId);
            Assert.Equal(valor, pagamento.Valor);
            Assert.Equal(FormaPagamento.QrCodeMercadoPago, pagamento.FormaPagamento);
            Assert.Equal(StatusPagamento.Aguardando, pagamento.StatusPagamento);
            Assert.True(pagamento.Ativo);
        }

        [Fact(DisplayName = "Criar pagamento com id com sucesso")]
        public void CriarPagamentoComCampoId_DeveRetornarSucesso()
        {
            //Arrange    
            var pagamentoId = Guid.NewGuid().ToString();
            var pedidoId = 1;
            var valor = 100;

            //Act 
            var pagamento = new Pagamento.Domain.Aggregates.Pagamento(pagamentoId, pedidoId, valor, StatusPagamento.Aguardando);

            //Assert
            Assert.NotNull(pagamento);
            Assert.Equal(pedidoId, pagamento.PedidoId);
            Assert.Equal(valor, pagamento.Valor);
            Assert.Equal(StatusPagamento.Aguardando, pagamento.StatusPagamento);
            Assert.True(pagamento.Ativo);
        }

        [Theory(DisplayName = "Criar pagamento com falha")]
        [InlineData(0, 100)]
        [InlineData(1, 0)]
        public void CriarPagamento_Invalido_DeveLancarException(int pedidoId, decimal valor)
        {
            //Arrange, Act & Assert
            Assert.Throws<DomainException>(() => new Pagamento.Domain.Aggregates.Pagamento(pedidoId, valor, FormaPagamento.QrCodeMercadoPago));
        }


        [Fact]
        public void Reprovar_PagamentoAguardando_DeveAlterarStatusParaReprovado()
        {
            // Arrange
            var pagamento = new Pagamento.Domain.Aggregates.Pagamento(1, 100, FormaPagamento.QrCodeMercadoPago);

            // Act
            pagamento.Reprovar();

            // Assert
            Assert.Equal(StatusPagamento.Reprovado, pagamento.StatusPagamento);
        }

        [Fact]
        public void Reprovar_PagamentoAprovado_DeveLancarExcecao()
        {
            // Arrange
            var pagamento = new Pagamento.Domain.Aggregates.Pagamento(1, 100, FormaPagamento.QrCodeMercadoPago);
            pagamento.Aprovar();

            // Act & Assert
            Assert.Throws<DomainException>(() => pagamento.Reprovar());
        }

        [Fact]
        public void Aprovar_Pagamento_DeveAlterarStatusParaAprovado()
        {
            // Arrange
            var pagamento = new Pagamento.Domain.Aggregates.Pagamento(1, 100, FormaPagamento.QrCodeMercadoPago);

            // Act
            pagamento.Aprovar();

            // Assert
            Assert.Equal(StatusPagamento.Aprovado, pagamento.StatusPagamento);
        }

        [Fact]
        public void ValorNegativo_DeveLancarExcecao()
        {
            // Act & Assert
            Assert.Throws<DomainException>(() => new Pagamento.Domain.Aggregates.Pagamento(1, -100, FormaPagamento.QrCodeMercadoPago));
        }

        [Fact]
        public void InativarPagamento()
        {
            //Arrange    
            var pedidoId = 1;
            var valor = 100;

            //Act 
            var pagamento = new Pagamento.Domain.Aggregates.Pagamento(pedidoId, valor, FormaPagamento.QrCodeMercadoPago);

            //Assert
            Assert.NotNull(pagamento);
            Assert.Equal(pedidoId, pagamento.PedidoId);
            Assert.Equal(valor, pagamento.Valor);
            Assert.Equal(FormaPagamento.QrCodeMercadoPago, pagamento.FormaPagamento);
            Assert.Equal(StatusPagamento.Aguardando, pagamento.StatusPagamento);
            Assert.True(pagamento.Ativo);

            pagamento.Inativar();

            Assert.False(pagamento.Ativo);
        }
    }
}
