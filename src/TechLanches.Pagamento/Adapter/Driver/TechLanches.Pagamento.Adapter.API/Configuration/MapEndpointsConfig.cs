using TechLanches.Pagamento.Adapter.API.Endpoints;

namespace TechLanches.Pagamento.Adapter.API.Configuration
{
    public static class MapEndpointsConfig
    {
        public static void UseMapEndpointsConfiguration(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPagamentoEndpoints();
        }
    }
}
