using System.Drawing;

namespace TechLanches.Pagamento.Application.Ports.Services.Interfaces
{
    public interface IQrCodeGeneratorService
    {
        public Bitmap GenerateImage(string pagamentoPedido);
        public byte[] GenerateByteArray(string url);
    }
}
