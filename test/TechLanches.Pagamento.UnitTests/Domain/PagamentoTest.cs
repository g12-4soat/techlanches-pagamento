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
    }
}
