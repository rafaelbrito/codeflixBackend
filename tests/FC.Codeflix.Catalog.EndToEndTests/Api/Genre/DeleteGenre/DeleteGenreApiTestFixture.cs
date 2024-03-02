using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.DeleteGenre
{
    [CollectionDefinition(nameof(DeleteGenreApiTestFixture))]
    public class DeleteGenreApiTestFixtureCollection:ICollectionFixture<DeleteGenreApiTestFixture>
    { }
    public class DeleteGenreApiTestFixture : GenreBaseFixture
    {
    }
}
