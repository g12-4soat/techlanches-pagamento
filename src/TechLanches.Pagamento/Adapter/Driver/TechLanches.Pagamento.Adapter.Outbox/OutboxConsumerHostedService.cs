using Microsoft.Extensions.Hosting;
using TechLanches.Pagamento.Adapter.Outbox.Services;

namespace TechLanches.Pagamento.Adapter.Outbox
{
    public class OutboxConsumerHostedService : BackgroundService
    {
        private readonly IOutboxService _outboxService;
        private const int DELAY_TIME = 10000;

        public OutboxConsumerHostedService(IOutboxService outboxService)
        {
            _outboxService = outboxService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _outboxService.ProcessMessages();
                await Task.Delay(DELAY_TIME, stoppingToken);
            }
        }
    }
}