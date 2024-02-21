using FC.Codeflix.Catalog.UniTests.Application.Genre.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Application.Genre.GetGenre
{
    [CollectionDefinition(nameof(GetGenreTestFixture))]
    public class GetGenreTestFixtureCollection : ICollectionFixture<GetGenreTestFixture>
    { }
    
    public class GetGenreTestFixture:GenreUseCasesBaseFixture
    {
    }
}
