using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.CreateGenre
{
    [CollectionDefinition(nameof(CreateGenreApiTestFixture))]
    public class CreateGenreApiTestFixtureCollection:ICollectionFixture<CreateGenreApiTestFixture>
    { }
    public class CreateGenreApiTestFixture: GenreBaseFixture
    {
        public CreateGenreInput GetExampleInput()
         => new CreateGenreInput(GetValidGenreName(),
             GetRandomBoolean()
             );

        public CreateGenreInput GetExampleInput(List<Guid>? listCategories)
        => new CreateGenreInput(GetValidGenreName(),
            GetRandomBoolean(),
            listCategories
            );
    }
}
