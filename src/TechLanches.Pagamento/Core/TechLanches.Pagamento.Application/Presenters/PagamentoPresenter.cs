using Mapster;
using TechLanches.Pagamento.Application.DTOs;
using TechLanches.Pagamento.Application.Presenters.Interfaces;

namespace TechLanches.Pagamento.Application.Presenters
{
    public class PagamentoPresenter : IPagamentoPresenter
    {
        public PagamentoResponseDTO ParaDto(Domain.Aggregates.Pagamento pagamento)
        {
            return pagamento.Adapt<PagamentoResponseDTO>();
        }
    }
}