using TechLanches.Pagamento.Application.DTOs;

namespace TechLanches.Pagamento.Application.Presenters.Interfaces
{
    public interface IPagamentoPresenter
    {
        PagamentoResponseDTO ParaDto(Domain.Aggregates.Pagamento pagamento);
    }
}
