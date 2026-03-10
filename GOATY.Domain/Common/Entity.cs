namespace GOATY.Domain.Common
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        private readonly List<DomainEvent> _domainEvents= [];

        protected Entity() { }
        protected Entity(Guid id)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
        }

        public void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
        public void RemoveDomainEvent(DomainEvent domainEven)
        {
            _domainEvents.Remove(domainEven);
        }
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
