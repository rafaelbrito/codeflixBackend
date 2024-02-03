using Bogus;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Validation;
using FluentAssertions;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Domain.Validation
{
    public class DomainValidationTest
    {
        public Faker Faker { get; set; } = new Faker();

        [Fact(DisplayName = nameof(NotNullOk))]
        [Trait("Domain", "DomainValidation - Validation")]
        public void NotNullOk()
        {
            var value = Faker.Commerce.ProductName();
            var fieldName = Faker.Commerce.ProductName();
            Action action = () => DomainValidation.NotNull(value, fieldName);
            action.Should().NotThrow();
        }

        [Fact(DisplayName = nameof(NotNullThrowWhenNull))]
        [Trait("Domain", "DomainValidation - Validation")]
        public void NotNullThrowWhenNull()
        {
            string? value = null;
            string fieldName = Faker.Commerce.ProductName();
            Action action = () => DomainValidation.NotNull(value, fieldName);
            action.Should().Throw<EntityValidationException>()
                .WithMessage($"{fieldName} should not be null");
        }
       
        [Theory(DisplayName = nameof(NotNullOrEmptyTrowWhenEmpty))]
        [Trait("Domain", "DomainValidation - Validation")]
        [InlineData("   ")]
        [InlineData(null)]
        [InlineData("")]
        public void NotNullOrEmptyTrowWhenEmpty(string? target)
        {
            string fieldName = Faker.Commerce.ProductName();
            Action action = () => DomainValidation.NotNullOrEmpty(target, fieldName);
            action.Should().Throw<EntityValidationException>()
              .WithMessage($"{fieldName} should not be null or empty");
        }

        [Fact(DisplayName = nameof(NotNullOrEmptyOk))]
        [Trait("Domain", "DomainValidation - Validation")]
        public void NotNullOrEmptyOk()
        {
            var target = Faker.Commerce.ProductName();
            var fieldName = Faker.Commerce.ProductName();
            Action action = () => DomainValidation.NotNullOrEmpty(target, fieldName);
            action.Should().NotThrow();
        }

        [Theory(DisplayName = nameof(MinLengthTrowWhenLess))]
        [Trait("Domain", "DomainValidation - Validation")]
        [MemberData(nameof(GetValuesSmallerThanThenMin), parameters: 10)]
        public void MinLengthTrowWhenLess(string target, int minLegth)
        {
            var fieldName = Faker.Commerce.ProductName();
            Action action = () => DomainValidation.MinLength(target, minLegth, fieldName);
            action.Should().Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should be at least {minLegth} characters long");
        }

        [Theory(DisplayName = nameof(MinLengthOk))]
        [Trait("Domain", "DomainValidation - Validation")]
        [MemberData(nameof(GetValuesGreaterThanThenMin), parameters: 10)]
        public void MinLengthOk(string target, int minLegth)
        {
            var fieldName = Faker.Commerce.ProductName();
            Action action = () => DomainValidation.MinLength(target, minLegth, fieldName);
            action.Should().NotThrow();
        }

        public static IEnumerable<object[]> GetValuesSmallerThanThenMin(int numberOfTests = 5)
        {
            var faker = new Faker();
            for (int i = 0; i < numberOfTests; i++)
            {
                var example = faker.Commerce.ProductName();
                var minLength = example.Length + (new Random().Next(1, 20));
                yield return new object[] {
                example,
                minLength
                };
            }
        }

        public static IEnumerable<object[]> GetValuesGreaterThanThenMin(int numberOfTests = 5)
        {
            var faker = new Faker();
            for (int i = 0; i < numberOfTests; i++)
            {
                var example = faker.Commerce.ProductName();
                var minLength = example.Length - (new Random().Next(1, 5));
                yield return new object[] {
                example,
                minLength
                };
            }
        }

        [Theory(DisplayName = nameof(GetValuesGreaterThanThenMax))]
        [Trait("Domain", "DomainValidation - Validation")]
        [MemberData(nameof(GetValuesGreaterThanThenMax), parameters: 10)]
        public void MaxLengthTrowWhenGreater(string target, int maxLegth)
        {
            var fieldName = Faker.Commerce.ProductName();
            Action action = () => DomainValidation.MaxLength(target, maxLegth, fieldName);

            action.Should().Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should be least or equal {maxLegth} characters long");
        }

        [Theory(DisplayName = nameof(MaxLengthOk))]
        [Trait("Domain", "DomainValidation - Validation")]
        [MemberData(nameof(GetValuesLessThanMax), parameters: 10)]
        public void MaxLengthOk(string target, int maxLegth)
        {
            var fieldName = Faker.Commerce.ProductName();
            Action action = () => DomainValidation.MaxLength(target, maxLegth, fieldName);
            action.Should().NotThrow();
        }


        public static IEnumerable<object[]> GetValuesLessThanMax(int numberOfTests = 5)
        {
            var faker = new Faker();
            for (int i = 0; i < numberOfTests; i++)
            {
                var example = faker.Commerce.ProductName();
                var maxLength = example.Length + (new Random().Next(0, 5));
                yield return new object[] {
                example,
                maxLength
                };
            }
        }

        public static IEnumerable<object[]> GetValuesGreaterThanThenMax(int numberOfTests = 5)
        {
            var faker = new Faker();
            for (int i = 0; i < numberOfTests; i++)
            {
                var example = faker.Commerce.ProductName();
                var maxLength = example.Length - (new Random().Next(1, 5));
                yield return new object[] {
                example,
                maxLength
                };
            }
        }
    }
}
