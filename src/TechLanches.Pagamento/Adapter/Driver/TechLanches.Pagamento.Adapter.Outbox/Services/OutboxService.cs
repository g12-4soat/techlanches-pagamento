
using Microsoft.Extensions.Options;
using TechLanches.Pagamento.Adapter.RabbitMq;
using TechLanches.Pagamento.Adapter.RabbitMq.Messaging;
using TechLanches.Pagamento.Adapter.RabbitMq.Options;
using TechLanches.Pagamento.Application.Ports.Repositories;
using TechLanches.Pagamento.Domain.Enums;

namespace TechLanches.Pagamento.Adapter.Outbox.Services
{
    public class OutboxService : IOutboxService
    {
        private readonly IOutboxRepository _outboxRepository;
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly IRabbitMqService _rabbitMqService;
        private readonly RabbitOptions _rabbitOptions;

        public OutboxService(IPagamentoRepository pagamentoRepository, IOptions<RabbitOptions> rabbitOptions,
            IOutboxRepository outboxRepository, IRabbitMqService rabbitMqService)
        {
            _pagamentoRepository = pagamentoRepository;
            _rabbitOptions = rabbitOptions.Value;
            _outboxRepository = outboxRepository;
            _rabbitMqService = rabbitMqService;
        }

        public async Task ProcessMessages()
        {
            var messages = await _outboxRepository.ObterNaoProcessados();

            foreach (var message in messages)
            {
                var pagamento = await _pagamentoRepository.BuscarPagamentoPorId(message.PagamentoId);
                var pagamentoFoiAprovado = pagamento.StatusPagamento == StatusPagamento.Aprovado;
                PedidoStatusMessage pedidoStatusMessage;

                if (pagamentoFoiAprovado)
                    pedidoStatusMessage = new PedidoStatusMessage(pagamento.PedidoId, Domain.Enums.Pedido.StatusPedido.PedidoRecebido);
                else
                    pedidoStatusMessage = new PedidoStatusMessage(pagamento.PedidoId, Domain.Enums.Pedido.StatusPedido.PedidoCanceladoPorPagamentoRecusado);

                _rabbitMqService.Publicar(pedidoStatusMessage, _rabbitOptions.QueueOrderStatus);

                var outbox = new Domain.Aggregates.Outbox(message.Id, pagamento.Id, "1");
                await _outboxRepository.ProcessarMensagem(outbox);
            }
        }
    }
}
