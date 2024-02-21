using FC.Codeflix.Catalog.UniTests.Application.Genre.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Application.Genre.DeleteGenre
{
    [CollectionDefinition(nameof(DeleteGenreTestFixture))]
    public class DeleteGenreTestFixtureCollection : ICollectionFixture<DeleteGenreTestFixture>
    { }

    public class DeleteGenreTestFixture : GenreUseCasesBaseFixture
    {
    }
}

