using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace TechLanches.Pagamento.Adapter.API.Configuration
{
    public static class HealthCheckConfig
    {
        public static void AddHealthCheckEndpoint(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = _ => true,
                });
            });
        }
    }
}