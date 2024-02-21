using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.Common;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.UpdateCategory
{
    [CollectionDefinition(nameof(UpdateCategoryTestFixture))]
    public class UpdateCategoryTestFixtureCollection : ICollectionFixture<UpdateCategoryTestFixture>
    { }

    public class UpdateCategoryTestFixture: CategoryUseCasesBaseFixture
    {
        public UpdateCategoryInput GetValidInput(Guid? id = null)
        => new(
            id ?? Guid.NewGuid(),
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
            );

        public UpdateCategoryInput GetInvalidInputShortName()
        {
            var invalidInputShortName = GetValidInput();
            invalidInputShortName.Name = invalidInputShortName.Name.Substring(0, 2);
            return invalidInputShortName;
        }

        public UpdateCategoryInput GetInvalitInputTooLongName()
        {
            var invalidLongName = GetValidInput();
            var tooLongNameCategory = Faker.Commerce.ProductName();
            while (tooLongNameCategory.Length <= 255)
                tooLongNameCategory = $"{tooLongNameCategory} {Faker.Commerce.ProductName()}";
            invalidLongName.Name = tooLongNameCategory;
            return invalidLongName;
        }

        public UpdateCategoryInput GetInvalidInputTooLongDescription()
        {
            var invalidLongDescriptionCategory = GetValidInput();
            var tooLongDescription = Faker.Commerce.ProductDescription();
            while (tooLongDescription.Length <= 10_000)
                tooLongDescription = $"{tooLongDescription} {Faker.Commerce.ProductDescription()}";
            invalidLongDescriptionCategory.Description = tooLongDescription;
            return invalidLongDescriptionCategory;
        }
    }
}
