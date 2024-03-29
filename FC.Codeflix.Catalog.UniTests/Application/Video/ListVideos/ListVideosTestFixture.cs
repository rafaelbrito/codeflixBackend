using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.UniTests.Common.Fixtures;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Application.Video.ListVideos
{
    [CollectionDefinition(nameof(ListVideosTestFixture))]
    public class ListVideoTestFixtureCollection : ICollectionFixture<ListVideosTestFixture>
    {

    }
    public class ListVideosTestFixture : VideoBaseTestFixture
    {

        private string GetValidCategoryName()
        {
            var categoryName = "";
            while (categoryName.Length < 3)
                categoryName = Faker.Commerce.Categories(1)[0];
            if (categoryName.Length > 255)
                categoryName = categoryName[..255];
            return categoryName;
        }

        private string GetValidCategoryDescription()
        {
            var categoryDescription = Faker.Commerce.ProductDescription();
            if (categoryDescription.Length > 255)
                categoryDescription = categoryDescription[..10_000];
            return categoryDescription;
        }

        private DomainEntity.Category GetExampleCategory()
           => new(GetValidCategoryName(),
           GetValidCategoryDescription(),
           GetRandomBoolean()
       );

        private string GetValidGenreName()
            => Faker.Commerce.Categories(1)[0];

        private DomainEntity.Genre GetExampleGenre()
        => new(
                GetValidGenreName(),
                GetRandomBoolean()
                );

        public (List<DomainEntity.Video>,
                List<DomainEntity.Category> Categories,
                List<DomainEntity.Genre> Genres)
            GetExampleVideoListWithRelations()
        {
            var itemsCreated = Random.Shared.Next(2, 10);
            var categories = new List<DomainEntity.Category>();
            var genres = new List<DomainEntity.Genre>();

            var videos = Enumerable.Range(1, itemsCreated)
            .Select(_ =>
            GetValidVideoWithAllProperties())
            .ToList();
            videos.ForEach(video =>
            {
                video.RemoveAllCategory();
                var qtdCategories = Random.Shared.Next(2, 5);
                for (int i = 0; i < qtdCategories; i++)
                {
                    var category = GetExampleCategory();
                    categories.Add(category);
                    video.AddCategory(category.Id);
                }

                video.RemoveAllGenres();
                var qtdGenres = Random.Shared.Next(2, 5);
                for (int i = 0; i < qtdGenres; i++)
                {
                    var genre = GetExampleGenre();
                    genres.Add(genre);
                    video.AddGenre(genre.Id);
                }
            });
            return (videos, categories, genres);
        }

        public List<DomainEntity.Video> GetExampleVideoListWithoutRelations()
            => Enumerable.Range(1, Random.Shared.Next(2, 10))
            .Select(_ =>
            GetValidVideo())
            .ToList();
    }
}
