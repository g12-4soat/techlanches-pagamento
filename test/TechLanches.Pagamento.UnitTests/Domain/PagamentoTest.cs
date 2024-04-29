using TechLanches.Pagamento.Core;
using TechLanches.Pagamento.Domain.Enums;

namespace TechLanches.Pagamento.UnitTests.Domain
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
        }

        [Theory(DisplayName = "Criar pagamento com falha")]
        [InlineData(0, 100)]
        [InlineData(1, 0)]
        public void CriarPagamento_Invalido_DeveLancarException(int pedidoId, decimal valor)
        {
            //Arrange, Act & Assert
            Assert.Throws<DomainException>(() => new Pagamento.Domain.Aggregates.Pagamento(pedidoId, valor, FormaPagamento.QrCodeMercadoPago));
        }
    }
}
