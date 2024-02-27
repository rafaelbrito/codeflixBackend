using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Xunit;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.ListGenres
{
    [CollectionDefinition(nameof(ListGenresTestFixture))]
    public class ListGenreTestFixrueCollection : ICollectionFixture<ListGenresTestFixture>
    { }
    public class ListGenresTestFixture : GenreUseCasesBaseFixture
    {
        

        public List<DomainEntity.Genre> CloneGenreListOrdered(
            List<DomainEntity.Genre> genreList, string orderBy, SearchOrder order)
        {
            var listClone = new List<DomainEntity.Genre>(genreList);
            var orderedEnumerable = (orderBy.ToLower(), order) switch
            {
                ("name", SearchOrder.Asc) => listClone.OrderBy(x => x.Name)
                    .ThenBy(x => x.Id),
                ("name", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Name)
                    .ThenByDescending(x => x.Id),
                ("id", SearchOrder.Asc) => listClone.OrderBy(x => x.Id),
                ("id", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
                ("createdat", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt),
                ("createdat", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt),
                _ => listClone.OrderBy(x => x.Name)
                    .ThenBy(x => x.Id),
            };
            return orderedEnumerable.ToList();
        }
    }
}
