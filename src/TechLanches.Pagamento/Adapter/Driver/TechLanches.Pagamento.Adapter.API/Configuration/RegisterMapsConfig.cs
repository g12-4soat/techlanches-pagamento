using Mapster;
using System.Reflection;
using TechLanches.Pagamento.Application.DTOs;

namespace TechLanches.Pagamento.Adapter.API.Configuration
{
    public static class RegisterMapsConfig
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public static void RegisterMaps(this IServiceCollection services)
        {
            TypeAdapterConfig<Domain.Aggregates.Pagamento, PagamentoResponseDTO>.NewConfig()
                .Map(dest => dest.Id, src => src.PedidoId)
                .Map(dest => dest.StatusPagamento, src => src.StatusPagamento.ToString());

            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
        }
    }
}