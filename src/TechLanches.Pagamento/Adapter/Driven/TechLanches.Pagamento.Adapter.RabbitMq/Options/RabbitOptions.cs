namespace TechLanches.Pagamento.Adapter.RabbitMq.Options
{
    public class RabbitOptions
    {
        public string Host { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Queue { get; set; }
        public string QueueOrderCreated { get; set; }
        public string QueueOrderStatus { get; set; }
    }
}
