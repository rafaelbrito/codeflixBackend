using FC.Codeflix.Catalog.IntegrationTests.Base;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.Common
{
    public class GenreUseCasesBaseFixture : BaseFixture
    {
        public List<DomainEntity.Genre> GetExampleGenresListByNames(List<string> names)
            => names.Select(name => GetExampleGenre(name: name)).ToList();

        public string GetValidGenreName()
           => Faker.Commerce.Categories(1)[0];


        public DomainEntity.Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null, string? name = null)
        {
            var genre = new DomainEntity.Genre(
                name ?? GetValidGenreName(),
                isActive ?? GetRandomBoolean()
                );
            categoriesIds?.ForEach(genre.AddCategory);
            return genre;
        }

        public bool GetRandomBoolean()
            => new Random().NextDouble() < 0.5;

        public List<Guid> GetRandomIdsList(int? count = null)
            => Enumerable.Range(1, count ?? (new Random().Next(1, 10)))
                .Select(_ => Guid.NewGuid())
                .ToList();

        public List<DomainEntity.Genre> GetExampleListGenres(int count = 10)
             => Enumerable
                .Range(1, count)
                .Select(_ => GetExampleGenre())
                .ToList();

        public List<DomainEntity.Category> GetExampleCategoryList(int length = 10)
         => Enumerable.Range(0, length)
            .Select(_ =>
            GetExampleCategory())
            .ToList();

        public string GetValidCategoryDescription()
        {
            var categoryDescription = Faker.Commerce.ProductDescription();
            if (categoryDescription.Length > 255)
                categoryDescription = categoryDescription[..10_000];
            return categoryDescription;
        }

        public string GetValidCategoryName()
        {
            var categoryName = "";
            while (categoryName.Length < 3)
                categoryName = Faker.Commerce.Categories(1)[0];
            if (categoryName.Length > 255)
                categoryName = categoryName[..255];
            return categoryName;
        }

        public DomainEntity.Category GetExampleCategory()
            => new(GetValidCategoryName(),
                  GetValidCategoryDescription(),
                  GetRandomBoolean());
    }
}
