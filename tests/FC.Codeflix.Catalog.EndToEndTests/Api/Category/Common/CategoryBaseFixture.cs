using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.EndToEndTests.Base;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common
{
    public class CategoryBaseFixture : BaseFixture
    {
        public CategoryPersistence Persistence;
        public CategoryBaseFixture()
            : base()
        {
            Persistence = new CategoryPersistence(CreateDbContext());
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

        public List<DomainEntity.Category> GetExampleCategoriesListWithNames(List<string> names)
            => names.Select(name =>
            {
                var category = GetExampleCategory();
                category.Update(name);
                return category;
            }).ToList();

        public string GetValidCategoryDescription()
        {
            var categoryDescription = Faker.Commerce.ProductDescription();
            if (categoryDescription.Length > 255)
                categoryDescription = categoryDescription[..10_000];
            return categoryDescription;
        }

        public bool GetRandomBoolean()
        {
            return new Random().Next(0, 2) == 0;
        }

        public DomainEntity.Category GetExampleCategory()
         => new(GetValidCategoryName(),
                GetValidCategoryDescription(),
                GetRandomBoolean()
        );

        public List<DomainEntity.Category> GetExampleCategoryList(int length = 10)
         => Enumerable.Range(0, length)
            .Select(_ =>
            GetExampleCategory())
            .ToList();

        public string GetInvalidTooShortName()
         => Faker.Commerce.ProductName().Substring(0, 2);

        public string GetInvalitInputTooLongName()
        {
            var tooLongNameCategory = Faker.Commerce.ProductName();
            while (tooLongNameCategory.Length <= 255)
                tooLongNameCategory = $"{tooLongNameCategory} {Faker.Commerce.ProductName()}";
            return tooLongNameCategory;
        }

        public string GetInvalidDescriptionTooLong()
        {
            var tooLongDescription = Faker.Commerce.ProductDescription();
            while (tooLongDescription.Length <= 10_000)
                tooLongDescription = $"{tooLongDescription} {Faker.Commerce.ProductDescription()}";
            return tooLongDescription;
        }
    }
}

