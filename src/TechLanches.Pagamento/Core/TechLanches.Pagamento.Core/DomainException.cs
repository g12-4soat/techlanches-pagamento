namespace TechLanches.Pagamento.Core
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        { }
    }
}
