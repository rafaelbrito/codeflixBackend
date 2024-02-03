namespace FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository
{
    public interface ISearchableRepository<Tagrregate>
        where Tagrregate : AggregateRoot
    {
        Task <SearchOutput<Tagrregate>> Search(SearchInput input, CancellationToken cancellationToken);
    }
}
