using FC.Codeflix.Catalog.UniTests.Application.Genre.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Application.Genre.UpdateGenre
{
    [CollectionDefinition(nameof(UpdateGenreTestFixture))]
    public class UpdateGenreFixtureCollection: ICollectionFixture<UpdateGenreTestFixture>
    { }

    public class UpdateGenreTestFixture : GenreUseCasesBaseFixture
    {
    }
}
