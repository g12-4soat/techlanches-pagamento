namespace TechLanches.Pagamento.Adapter.RabbitMq.Messaging
{
    public interface IRabbitMqService
    {
        void Publicar(IBaseMessage baseMessage, string queueName);
        Task Consumir(Func<PedidoCriadoMessage, Task> function);
    }
}
