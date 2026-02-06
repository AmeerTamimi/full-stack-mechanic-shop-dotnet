namespace GOATY.Domain.Common
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        private readonly List<DomainEvent> _domainEvents;

        protected Entity() { }
        protected Entity(Guid id)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
        }

        public void AddEvent(DomainEvent domainEvent){
            _domainEvents.Add(domainEvent);
        }
        public void RemoveEvent(DomainEvent domainEven)
        {
            _domainEvents.Remove(domainEven);
        }
        public void ClearEvents()
        {
            _domainEvents.Clear();
        }
    }
}
