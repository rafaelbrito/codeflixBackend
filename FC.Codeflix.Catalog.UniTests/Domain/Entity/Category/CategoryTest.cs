using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
namespace FC.Codeflix.Catalog.UniTests.Domain.Entity.Category
{
    [Collection(nameof(CategoryTestFixture))]
    public class CategoryTest
    {
        private readonly CategoryTestFixture _categoryTestFixture;
        public CategoryTest(CategoryTestFixture categoryTestFixture)
        => _categoryTestFixture = categoryTestFixture;

        [Fact(DisplayName = nameof(Instantiate))]
        [Trait("Domain", "Category - Aggregates")]
        public void Instantiate()
        {
            var validCategory = _categoryTestFixture.GetValidCategory();
            var datetimeBefore = DateTime.Now;

            var category = new DomainEntity.Category(validCategory.Name, validCategory.Description);
            var datetimeAfter = DateTime.Now.AddSeconds(1);


            category.Should().NotBeNull();
            category.Name.Should().Be(validCategory.Name);
            category.Description.Should().Be(validCategory.Description);
            category.Id.Should().NotBeEmpty();
            category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
            (category.CreatedAt >= datetimeBefore).Should().BeTrue();
            (category.CreatedAt <= datetimeAfter).Should().BeTrue();
            (category.IsActive).Should().BeTrue();
        }

        [Theory(DisplayName = nameof(InstantiateWithIsActive))]
        [Trait("Domain", "Category - Aggregates")]
        [InlineData(true)]
        [InlineData(false)]
        public void InstantiateWithIsActive(bool isActive)
        {
            var validCategory = _categoryTestFixture.GetValidCategory();

            var datetimeBefore = DateTime.Now;

            var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, isActive);
            var datetimeAfter = DateTime.Now;

            category.Should().NotBeNull();
            category.Name.Should().Be(validCategory.Name);
            category.Description.Should().Be(validCategory.Description);
            category.Id.Should().NotBeEmpty();
            category.CreatedAt.Should().NotBeSameDateAs(default);
            (category.CreatedAt > datetimeBefore).Should().BeTrue();
            (category.CreatedAt < datetimeAfter).Should().BeTrue();
            (category.IsActive).Should().Be(isActive);
        }

        [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmpty))]
        [Trait("Domain", "Category - Aggregates")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void InstantiateErrorWhenNameIsEmpty(string? name)
        {
            var validCategory = _categoryTestFixture.GetValidCategory();
            Action action = () => new DomainEntity.Category(name!, validCategory.Description);
            action.Should().Throw<EntityValidationException>()
                .WithMessage("Name should not be null or empty"); ;
        }

        [Fact(DisplayName = nameof(InstantiateErrorWhenDescrptionIsNull))]
        [Trait("Domain", "Category - Aggregates")]
        public void InstantiateErrorWhenDescrptionIsNull()
        {
            var validCategory = _categoryTestFixture.GetValidCategory();
            Action action = () => new DomainEntity.Category(validCategory.Name, null!);
            action.Should().Throw<EntityValidationException>()
                .WithMessage("Description should not be null");
        }

        [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsLessThan3Characters))]
        [Trait("Domain", "Category - Aggregates")]
        [MemberData(nameof(GetNamesWhithLessThan3Characters), parameters: 15)]
        public void InstantiateErrorWhenNameIsLessThan3Characters(string invalidName)
        {
            var validCategory = _categoryTestFixture.GetValidCategory();
            Action action = () => new DomainEntity.Category(invalidName, validCategory.Description);
            action.Should().Throw<EntityValidationException>()
                .WithMessage("Name should be at least 3 characters long");
        }

        public static IEnumerable<object[]> GetNamesWhithLessThan3Characters(int numberOfTests = 6)
        {
            var fixture = new CategoryTestFixture();
            for (int i = 0; i < numberOfTests; i++)
            {
                var isOdd = i % 2 == 1;
                yield return new object[] {
                    fixture.GetValidCategoryName()[..(isOdd ? 1 : 2)] };
            };
        }

        [Fact(DisplayName = nameof(InstantiateErrorWhenNameIsGreaterThan255Chacacters))]
        [Trait("Domain", "Category - Aggregates")]
        public void InstantiateErrorWhenNameIsGreaterThan255Chacacters()
        {
            var validCategory = _categoryTestFixture.GetValidCategory();
            var invalidName = String.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());
            Action action = () => new DomainEntity.Category(invalidName, validCategory.Description);
            action.Should().Throw<EntityValidationException>()
              .WithMessage("Name should be least or equal 255 characters long");
        }

        [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThan10_000Chacacters))]
        [Trait("Domain", "Category - Aggregates")]
        public void InstantiateErrorWhenDescriptionIsGreaterThan10_000Chacacters()
        {
            var validCategory = _categoryTestFixture.GetValidCategory();
            var invalidDescription = String.Join(null, Enumerable.Range(1, 10_001).Select(_ => "a").ToArray());
            Action action = () => new DomainEntity.Category(validCategory.Name, invalidDescription);
            action.Should().Throw<EntityValidationException>()
                .WithMessage("Description should be least or equal 10000 characters long");
        }

        [Fact(DisplayName = nameof(Activate))]
        [Trait("Domain", "Category - Aggregates")]
        public void Activate()
        {
            var validCategory = _categoryTestFixture.GetValidCategory();

            var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, false);

            category.Activate();
            category.IsActive.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(Desactivate))]
        [Trait("Domain", "Category - Aggregates")]
        public void Desactivate()
        {
            var validCategory = _categoryTestFixture.GetValidCategory();
            var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, true);

            category.Deactivate();
            category.IsActive.Should().BeFalse();
        }

        [Fact(DisplayName = nameof(Update))]
        [Trait("Domain", "Category - Aggregates")]
        public void Update()
        {
            var category = _categoryTestFixture.GetValidCategory();
            var newValues = _categoryTestFixture.GetValidCategory(); ;
            category.Update(newValues.Name, newValues.Description);
            category.Name.Should().Be(newValues.Name);
            category.Description.Should().Be(newValues.Description);
        }

        [Fact(DisplayName = nameof(UpdateOnlyName))]
        [Trait("Domain", "Category - Aggregates")]
        public void UpdateOnlyName()
        {
            var category = _categoryTestFixture.GetValidCategory();
            var newName = _categoryTestFixture.GetValidCategoryName();
            var currentDescription = category.Description;
            category.Update(newName);

            category.Name.Should().Be(newName);
            category.Description.Should().Be(currentDescription);
        }

        [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmpty))]
        [Trait("Domain", "Category - Aggregates")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void UpdateErrorWhenNameIsEmpty(string? name)
        {
            var category = _categoryTestFixture.GetValidCategory();
            Action action = () => category.Update(name!);
            action.Should().Throw<EntityValidationException>()
                .Which.Message.Should().Contain("Name should not be null or empty");
        }

        [Theory(DisplayName = nameof(UpdateErrorWhenNameIsLessThan3Characters))]
        [Trait("Domain", "Category - Aggregates")]
        [MemberData(nameof(GetNamesWhithLessThan3Characters), parameters: 10)]
        public void UpdateErrorWhenNameIsLessThan3Characters(string invalidName)
        {
            var category = _categoryTestFixture.GetValidCategory();
            Action action = () => category.Update(invalidName);
            action.Should().Throw<EntityValidationException>()
                .Which.Message.Should().Contain("Name should be at least 3 characters long");
        }

        [Fact(DisplayName = nameof(UpdateErrorWhenNameIsGreaterThan255Chacacters))]
        [Trait("Domain", "Category - Aggregates")]
        public void UpdateErrorWhenNameIsGreaterThan255Chacacters()
        {
            var category = _categoryTestFixture.GetValidCategory();
            var invalidName = _categoryTestFixture.Faker.Lorem.Letter(256);

            Action action = () => category.Update(invalidName);
            action.Should().Throw<EntityValidationException>()
            .Which.Message.Should().Contain("Name should be least or equal 255 characters long");
        }

        [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThan10_000Chacacters))]
        [Trait("Domain", "Category - Aggregates")]
        public void UpdateErrorWhenDescriptionIsGreaterThan10_000Chacacters()
        {
            var category = _categoryTestFixture.GetValidCategory();
            var invalidDescription = _categoryTestFixture.Faker.Commerce.ProductDescription();
            while (invalidDescription.Length <= 10_000)
                invalidDescription = $"{invalidDescription} {_categoryTestFixture.Faker.Commerce.ProductDescription()}";

            Action action = () => category.Update("New Name", invalidDescription);
            action.Should().Throw<EntityValidationException>()
              .Which.Message.Should().Contain("Description should be least or equal 10000 characters long");
        }
    }
}

