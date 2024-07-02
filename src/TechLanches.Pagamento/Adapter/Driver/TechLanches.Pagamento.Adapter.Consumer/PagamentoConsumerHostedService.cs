using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechLanches.Pagamento.Adapter.RabbitMq.Messaging;
using TechLanches.Pagamento.Application.Controllers;

namespace TechLanches.Pagamento.Adapter.Consumer
{
    public class PagamentoConsumerHostedService : BackgroundService
    {
        private readonly IPagamentoController _pagamentoController;
        private readonly IRabbitMqService _rabbitMqService;

        public PagamentoConsumerHostedService(IPagamentoController pagamentoController, IRabbitMqService rabbitMqService)
        {
            _pagamentoController = pagamentoController;
            _rabbitMqService = rabbitMqService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _rabbitMqService.Consumir(_pagamentoController.ProcessarMensagem);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
