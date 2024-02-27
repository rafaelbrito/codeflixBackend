using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.UpdateGenre
{
    [CollectionDefinition(nameof(UpdateGenreTestFixture))]
    public class UpdateGenreTestFixrueCollection : ICollectionFixture<UpdateGenreTestFixture>
    { }
    public class UpdateGenreTestFixture: GenreUseCasesBaseFixture
    {
    }
}
