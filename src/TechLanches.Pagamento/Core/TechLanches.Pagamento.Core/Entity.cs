namespace TechLanches.Pagamento.Core
{
    public abstract class Entity
    {
        protected Entity()
        {
            Id = Guid.NewGuid().ToString();
        }

        protected Entity(string id)
        {
            Id = id;
        }

        public string Id { get; private set; }

    }
}
