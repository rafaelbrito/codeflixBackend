using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.Codeflix.Catalog.UniTests.Application.Genre.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Application.Genre.CreateGenre
{
    [CollectionDefinition(nameof(CreateGenreTestFixture))]
    public class CreateGenreTestFixtureCollection : ICollectionFixture<CreateGenreTestFixture>
    { }

    public class CreateGenreTestFixture : GenreUseCasesBaseFixture
    {

        public CreateGenreInput GetExampleInput()
            => new CreateGenreInput(
                   GetValidGenreName(),
                   GetRandomBoolean()
                    );

        public CreateGenreInput GetExampleInput(string? name)
            => new CreateGenreInput(
              name,
              GetRandomBoolean()
               );

        public CreateGenreInput GetExampleInputWithCategories()
        {
            var numberOfCategoriesIds = (new Random()).Next(1, 10);
            var categoriesIds = Enumerable.Range(1, numberOfCategoriesIds)
                .Select(_ => Guid.NewGuid())
                .ToList();
            return new CreateGenreInput(
                        GetValidGenreName(),
                        GetRandomBoolean(),
                        categoriesIds
                        );
        }


    }
}
