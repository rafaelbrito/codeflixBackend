namespace FC.Codeflix.Catalog.Application.Inferfaces
{
    public interface IUnitOfWork
    {
        public Task Commit(CancellationToken cancellationToken);

        public Task Rollback(CancellationToken cancellationToken);
    }
}
