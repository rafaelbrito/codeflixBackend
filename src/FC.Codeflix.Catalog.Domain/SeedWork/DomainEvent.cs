namespace FC.Codeflix.Catalog.Domain.SeedWork
{
    public abstract class DomainEvent
    {
        public DateTime OcurredOn { get; set; }

        protected DomainEvent()
            => OcurredOn = DateTime.Now;
    }
}
