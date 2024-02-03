using Xunit;
using FluentAssertions;

namespace FC.Codeflix.Catalog.UniTests.Application.Category.UpdateCategory
{
    [Collection(nameof(UpdateCategoryTestFixture))]
    public class UpdateCategoryInputValidatorTest
    {
        private readonly UpdateCategoryTestFixture _fixture;
        public UpdateCategoryInputValidatorTest(UpdateCategoryTestFixture fixture)
         => _fixture = fixture;

        [Fact(DisplayName = nameof(DontValidateWhwenGuid))]
        [Trait("Application", "UpdateCategoryInputValidator - Use Cases")]
        public void DontValidateWhwenGuid()
        {
            var input = _fixture.GetValidInput(Guid.Empty);
            var validator = new UpdateCategoryInputValidator();

            var validResult = validator.Validate(input);

            validResult.Should().NotBeNull();
            validResult.IsValid.Should().BeFalse();
            validResult.Errors.Should().HaveCount(1);
            validResult.Errors[0].ErrorMessage.Should().Be("'Id' must not be empty.");
        }

        [Fact(DisplayName = nameof(ValidateWhwenValid))]
        [Trait("Application", "UpdateCategoryInputValidator - Use Cases")]
        public void ValidateWhwenValid()
        {
            var input = _fixture.GetValidInput();
            var validator = new UpdateCategoryInputValidator();

            var validResult = validator.Validate(input);

            validResult.Should().NotBeNull();
            validResult.IsValid.Should().BeTrue();
            validResult.Errors.Should().HaveCount(0);
        }
    }
}
