using TechLanches.Pagamento.Adapter.ACL.QrCode.DTOs;
using TechLanches.Pagamento.Adapter.ACL.QrCode.Provedores.MercadoPago;

namespace TechLanches.Pagamento.UnitTests.UnitTests.Adapter.ACL
{
    public class MercadoPagoMockadoServiceTests
    {
        [Fact]
        public async Task ConsultarPagamento_DeveRetornarPagamentoComStatusValido()
        {
            // Arrange
            var service = new MercadoPagoMockadoService();
            var idPagamentoComercial = "12345";

            // Act
            var pagamentoResponse = await service.ConsultarPagamento(idPagamentoComercial);

            // Assert
            Assert.NotNull(pagamentoResponse);
            Assert.True(Enum.IsDefined(typeof(StatusPagamentoEnum), pagamentoResponse.StatusPagamento));
        }

        [Fact]
        public async Task GerarPagamentoEQrCode_DeveRetornarQrCodeValido()
        {
            // Arrange
            var service = new MercadoPagoMockadoService();

            // Act
            var qrCode = await service.GerarPagamentoEQrCode();

            // Assert
            Assert.NotNull(qrCode);
            Assert.IsType<string>(qrCode);
        }

    }
}