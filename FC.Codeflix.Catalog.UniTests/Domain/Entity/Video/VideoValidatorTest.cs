using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Validation;
using FC.Codeflix.Catalog.Domain.Validator;
using FluentAssertions;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Domain.Entity.Video
{
    [Collection(nameof(VideoTestFixture))]
    public class VideoValidatorTest
    {
        private readonly VideoTestFixture _fixture;
        public VideoValidatorTest(VideoTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(ReturnsValidWhenVideoIsValid))]
        [Trait("Domain", "Video Validator - Validators")]
        public void ReturnsValidWhenVideoIsValid()
        {
            var validVideo = _fixture.GetValidVideo();
            var notificationValidationHandler = new NotificationValidationHandler();
            var videoValidator = new VideoValidator(validVideo, notificationValidationHandler);

            videoValidator.Validate();
            notificationValidationHandler.HasErrors().Should().BeFalse();
            notificationValidationHandler.Errors.Should().HaveCount(0);
        }

        [Fact(DisplayName = nameof(ReturnsErrorWhenTitleIsLong))]
        [Trait("Domain", "Video Validator - Validators")]
        public void ReturnsErrorWhenTitleIsLong()
        {
            var invalidVideo = new DomainEntity.Video(
                _fixture.GetTooLongTitle(),
                _fixture.GetValidVideoDescription(),
                _fixture.GetValidYearLauched(),
                _fixture.GetRandomBoolean(),
                _fixture.GetRandomBoolean(),
                _fixture.GetValidVideoDuration(),
                _fixture.GetRandomRating()
                );
            var notificationValidationHandler = new NotificationValidationHandler();
            var videoValidator = new VideoValidator(invalidVideo, notificationValidationHandler);

            videoValidator.Validate();
            notificationValidationHandler.HasErrors().Should().BeTrue();
            notificationValidationHandler.Errors.Should().HaveCount(1);
            notificationValidationHandler.Errors.ToList().First()
                .Message.Should().Be("'Title' should be less or equal 255 characters long");
        }

        [Theory(DisplayName = nameof(ReturnsErrorWhenTitleIsEmpty))]
        [Trait("Domain", "Video Validator - Validators")]
        [InlineData("")]
        [InlineData("   ")]
        public void ReturnsErrorWhenTitleIsEmpty(string title)
        {
            var invalidVideo = new DomainEntity.Video(
                 title,
                _fixture.GetValidVideoDescription(),
                _fixture.GetValidYearLauched(),
                _fixture.GetRandomBoolean(),
                _fixture.GetRandomBoolean(),
                _fixture.GetValidVideoDuration(),
                _fixture.GetRandomRating()
                );
            var notificationValidationHandler = new NotificationValidationHandler();
            var videoValidator = new VideoValidator(invalidVideo, notificationValidationHandler);

            videoValidator.Validate();
            notificationValidationHandler.HasErrors().Should().BeTrue();
            notificationValidationHandler.Errors.Should().HaveCount(1);
            notificationValidationHandler.Errors.ToList().First()
                .Message.Should().Be("'Title' is required");
        }

        [Theory(DisplayName = nameof(ReturnsErrorWhenDescriptionIsEmpty))]
        [Trait("Domain", "Video Validator - Validators")]
        [InlineData("")]
        [InlineData("   ")]
        public void ReturnsErrorWhenDescriptionIsEmpty(string description)
        {
            var invalidVideo = new DomainEntity.Video(
                 _fixture.GetValidVideoTitle(),
                 description,
                _fixture.GetValidYearLauched(),
                _fixture.GetRandomBoolean(),
                _fixture.GetRandomBoolean(),
                _fixture.GetValidVideoDuration(),
                _fixture.GetRandomRating()
                );
            var notificationValidationHandler = new NotificationValidationHandler();
            var videoValidator = new VideoValidator(invalidVideo, notificationValidationHandler);

            videoValidator.Validate();
            notificationValidationHandler.HasErrors().Should().BeTrue();
            notificationValidationHandler.Errors.Should().HaveCount(1);
            notificationValidationHandler.Errors.ToList().First()
                .Message.Should().Be("'Description' is required");
        }

        [Fact(DisplayName = nameof(ReturnsErrorWhenDescriptionIsLong))]
        [Trait("Domain", "Video Validator - Validators")]
        public void ReturnsErrorWhenDescriptionIsLong()
        {
            var invalidVideo = new DomainEntity.Video(
                 _fixture.GetValidVideoTitle(),
                 _fixture.GetTooLongDescription(),
                _fixture.GetValidYearLauched(),
                _fixture.GetRandomBoolean(),
                _fixture.GetRandomBoolean(),
                _fixture.GetValidVideoDuration(),
                _fixture.GetRandomRating()
                );
            var notificationValidationHandler = new NotificationValidationHandler();
            var videoValidator = new VideoValidator(invalidVideo, notificationValidationHandler);

            videoValidator.Validate();
            notificationValidationHandler.HasErrors().Should().BeTrue();
            notificationValidationHandler.Errors.Should().HaveCount(1);
            notificationValidationHandler.Errors.ToList().First()
                .Message.Should().Be("'Description' should be less or equal 4000 characters long");
        }
    }
}
