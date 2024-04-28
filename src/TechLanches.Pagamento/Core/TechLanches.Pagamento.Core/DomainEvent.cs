namespace TechLanches.Pagamento.Core
{
    public abstract class DomainEvent
    {
        protected DateTime DataCriacao { get; private set; }

        protected DomainEvent(DateTime dataCriacao)
        {
            DataCriacao = dataCriacao;
        }
    }
}
