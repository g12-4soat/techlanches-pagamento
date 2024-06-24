namespace TechLanches.Pagamento.Domain.Aggregates
{
    public class Outbox
    {
        public Outbox()
        {
            
        }
        public Outbox(string id, string pagamentoId, string processado)
        {
            Id = id;
            PagamentoId = pagamentoId;
            Processado = processado;
        }

        public string Id { get; set; }
        public string PagamentoId { get; set; }
        public string Processado { get; set; }
    }
}
