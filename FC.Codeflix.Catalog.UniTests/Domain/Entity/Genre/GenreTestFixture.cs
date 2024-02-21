using FC.Codeflix.Catalog.UniTests.Comon;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Domain.Entity.Genre
{
    [CollectionDefinition(nameof(GenreTestFixture))]
    public class GenreTestFixtureCollection : ICollectionFixture<GenreTestFixture>
    { }


    public class GenreTestFixture : BaseFixture
    {
        public string GetValidName()
            => Faker.Commerce.Categories(1)[0];

        public DomainEntity.Genre GetExampleGenre(
            bool isActive = true,
            List<Guid>?
            categoriesIdList = null
            )
        {
            var genre = new DomainEntity.Genre(GetValidName(), isActive);
            if (categoriesIdList is not null)
            {
                foreach(var categoryId in categoriesIdList)
                    genre.AddCategory(categoryId);
            }
            return genre;
        }
    }
}
