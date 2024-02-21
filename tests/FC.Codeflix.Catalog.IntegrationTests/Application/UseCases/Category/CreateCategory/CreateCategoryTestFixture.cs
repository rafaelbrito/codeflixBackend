using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.Common;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.CreateCategory
{
    [CollectionDefinition(nameof(CreateCategoryTestFixture))]
    public class CreateCategoryTestFixtureCollection: ICollectionFixture<CreateCategoryTestFixture>
    { }

    public class CreateCategoryTestFixture : CategoryUseCasesBaseFixture
    {
        public CreateCategoryInput GetInput()
        {
            var category = GetExampleCategory();
            return new CreateCategoryInput(
                    category.Name,
                    category.Description,
                    category.IsActive
                   );
        }
        public CreateCategoryInput GetInvalidInputShortName()
        {
            var invalidInputShortName = GetInput();
            invalidInputShortName.Name = invalidInputShortName.Name.Substring(0, 2);
            return invalidInputShortName;
        }

        public CreateCategoryInput GetInvalitInputTooLongName()
        {
            var invalidLongName = GetInput();
            var tooLongNameCategory = Faker.Commerce.ProductName();
            while (tooLongNameCategory.Length <= 255)
                tooLongNameCategory = $"{tooLongNameCategory} {Faker.Commerce.ProductName()}";
            invalidLongName.Name = tooLongNameCategory;
            return invalidLongName;
        }

        public CreateCategoryInput GetInvalidInputCategoryNull()
        {
            var invalidDescriptionNull = GetInput();
            invalidDescriptionNull.Description = null!;
            return invalidDescriptionNull;
        }

        public CreateCategoryInput GetInvalidInputTooLongDescription()
        {
            var invalidLongDescriptionCategory = GetInput();
            var tooLongDescription = Faker.Commerce.ProductDescription();
            while (tooLongDescription.Length <= 10_000)
                tooLongDescription = $"{tooLongDescription} {Faker.Commerce.ProductDescription()}";
            invalidLongDescriptionCategory.Description = tooLongDescription;
            return invalidLongDescriptionCategory;
        }
    }
}
