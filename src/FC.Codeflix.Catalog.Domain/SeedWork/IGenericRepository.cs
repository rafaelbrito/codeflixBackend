namespace FC.Codeflix.Catalog.Domain.SeedWork
{
    public interface IGerenericRepository<TAggregate>: IRepository
        where TAggregate : AggregateRoot
    {
        public Task Insert(TAggregate agreggate, CancellationToken cancellationToken);

        public Task<TAggregate> Get(Guid id, CancellationToken cancellationToken);

        public Task Delete(TAggregate agreggate, CancellationToken cancellationToken);

        public Task Update(TAggregate agreggate, CancellationToken cancellationToken);
    }
}
