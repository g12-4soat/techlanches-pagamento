using System.Text.Json;

namespace TechLanches.Pagamento.Adapter.RabbitMq
{
    public class PedidoMessage : IBaseMessage
    {
        public int Id { get; private set; }
        public string Cpf { get; private set; }

        public string Type => nameof(PedidoMessage);

        public PedidoMessage(int id, string cpf)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(cpf);

            Id = id;
            Cpf = cpf;
        }

        public string GetMessage()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
