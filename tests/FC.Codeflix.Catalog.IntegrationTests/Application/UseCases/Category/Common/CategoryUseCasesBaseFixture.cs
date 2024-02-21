using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.IntegrationTests.Base;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.Common
{
    public class CategoryUseCasesBaseFixture : BaseFixture
    {
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
    }
}
