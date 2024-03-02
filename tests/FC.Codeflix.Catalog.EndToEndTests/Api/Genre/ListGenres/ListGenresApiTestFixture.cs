using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.ListGenres
{
    [CollectionDefinition(nameof(ListGenresApiTestFixture))]
    public class ListGenresApiTestFixtureCollection : ICollectionFixture<ListGenresApiTestFixture>
    { }
    public class ListGenresApiTestFixture:GenreBaseFixture
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
