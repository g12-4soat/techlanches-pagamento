using Amazon.DynamoDBv2;
using TechLanches.Pagamento.Adapter.ACL.QrCode.Provedores.MercadoPago;
using TechLanches.Pagamento.Adapter.DynamoDB.Repositories;
using TechLanches.Pagamento.Adapter.RabbitMq.Messaging;
using TechLanches.Pagamento.Application.Controllers;
using TechLanches.Pagamento.Application.Gateways;
using TechLanches.Pagamento.Application.Gateways.Interfaces;
using TechLanches.Pagamento.Application.Ports.Repositories;
using TechLanches.Pagamento.Application.Presenters;
using TechLanches.Pagamento.Application.Presenters.Interfaces;

namespace TechLanches.Pagamento.Adapter.API.Configuration
{
    public static class DependecyInjectionConfig
    {
        public static void AddDependencyInjectionConfiguration(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));


            services.AddSingleton<IPagamentoPresenter, PagamentoPresenter>();

            services.AddSingleton<IPagamentoController, PagamentoController>();
            services.AddSingleton<IPedidoController, PedidoController>();

            services.AddScoped<IPedidoGateway, PedidoGateway>();

            services.AddScoped<IMercadoPagoMockadoService, MercadoPagoMockadoService>();
            services.AddSingleton<IMercadoPagoMockadoService, MercadoPagoMockadoService>();

            services.AddSingleton<IPagamentoRepository, PagamentoRepository>();

            services.AddSingleton<IRabbitMqService, RabbitMqService>();

            services.AddSingleton<IAmazonDynamoDB>(sp =>
            {
                var config = new AmazonDynamoDBConfig
                {
                    RegionEndpoint = Amazon.RegionEndpoint.USEast1
                };
                return new AmazonDynamoDBClient(config);
            });
        }
    }
}