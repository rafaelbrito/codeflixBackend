using System.Collections.ObjectModel;

namespace FC.Codeflix.Catalog.Domain.SeedWork
{
    public abstract class AggregateRoot : Entity
    {
        private readonly List<DomainEvent> _events = new();
        public IReadOnlyCollection<DomainEvent> Events 
            => new ReadOnlyCollection<DomainEvent>(_events);

        protected AggregateRoot() : base() { }

        public void RaiseEvent(DomainEvent @envent) => _events.Add(envent);

        public void ClearEvents() => _events.Clear();
    }
}
