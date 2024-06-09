namespace TechLanches.Pagamento.Adapter.RabbitMq
{
    public interface IBaseMessage
    {
        string Type { get; }
        string GetMessage();
    }
}
