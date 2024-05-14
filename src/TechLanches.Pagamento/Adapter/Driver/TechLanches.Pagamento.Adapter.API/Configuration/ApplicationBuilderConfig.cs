using TechLanches.Pagamento.Adapter.API.Middlewares;

namespace TechLanches.Pagamento.Adapter.API.Configuration
{
    public static class ApplicationBuilderConfig
    {
        public static IApplicationBuilder AddCustomMiddlewares(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<GlobalErrorHandlingMiddleware>();

            return applicationBuilder;
        }
    }
}