using FC.Codeflix.Catalog.Application.Inferfaces;

namespace FC.Codeflix.Catalog.Infra.Data.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CodeflixCatalogDbContext _context;
        public UnitOfWork(CodeflixCatalogDbContext context)
        => _context = context;

        public Task Commit(CancellationToken cancellationToken)
        => _context.SaveChangesAsync(cancellationToken);

        public Task Rollback(CancellationToken cancellationToken)
        => Task.CompletedTask;
    }
}
