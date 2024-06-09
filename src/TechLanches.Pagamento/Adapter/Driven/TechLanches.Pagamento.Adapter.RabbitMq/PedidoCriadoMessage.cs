using System.Text.Json;

namespace TechLanches.Pagamento.Adapter.RabbitMq
{
    public class PedidoCriadoMessage : IBaseMessage
    {
        public int Id { get; private set; }
        public decimal Valor { get; private set; }

        public string Type => nameof(PedidoCriadoMessage);

        public PedidoCriadoMessage(int id, decimal valor)
        {
            Id = id;
            Valor = valor;
        }

        public string GetMessage()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
