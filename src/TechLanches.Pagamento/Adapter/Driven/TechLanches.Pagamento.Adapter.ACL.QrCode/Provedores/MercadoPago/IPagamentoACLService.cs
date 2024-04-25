using TechLanches.Pagamento.Adapter.ACL.QrCode.DTOs;

namespace TechLanches.Pagamento.Adapter.ACL.QrCode.Provedores.MercadoPago
{
    public interface IPagamentoACLService
    {
        Task<string> GerarPagamentoEQrCode();
        Task<PagamentoResponseACLDTO> ConsultarPagamento(string idPagamentoComercial);
    }
}