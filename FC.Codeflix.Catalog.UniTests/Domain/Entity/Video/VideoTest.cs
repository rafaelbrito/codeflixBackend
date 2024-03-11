using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Validation;
using FluentAssertions;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UniTests.Domain.Entity.Video
{
    [Collection(nameof(VideoTestFixture))]
    public class VideoTest
    {
        private readonly VideoTestFixture _fixture;

        public VideoTest(VideoTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(Instantiate))]
        [Trait("Domain", "Video - Aggregate")]
        public void Instantiate()
        {
            var title = _fixture.GetValidVideoTitle();
            var description = _fixture.GetValidVideoDescription();
            var year = _fixture.GetValidYearLauched();
            var opened = _fixture.GetRandomBoolean();
            var published = _fixture.GetRandomBoolean();
            var duration = _fixture.GetValidVideoDuration();
            var rating = _fixture.GetRandomRating();

            var video = new DomainEntity.Video(title, description, year, opened, published, duration, rating);

            video.Title.Should().Be(title);
            video.Description.Should().Be(description);
            video.Opened.Should().Be(opened);
            video.Published.Should().Be(published);
            video.YearLauched.Should().Be(year);
            video.Duration.Should().Be(duration);
            video.Rating.Should().Be(rating);
            video.Thumb.Should().BeNull();
            video.ThumbHalf.Should().BeNull();
            video.Banner.Should().BeNull();
            video.Media.Should().BeNull();
            video.Trailer.Should().BeNull();
        }

        [Fact(DisplayName = nameof(ValidateWhenValidState))]
        [Trait("Domain", "Video - Aggregate")]
        public void ValidateWhenValidState()
        {

            var exampleVideo = _fixture.GetValidVideo();
            var notificationHandler = new NotificationValidationHandler();
            exampleVideo.Validate(notificationHandler);

            notificationHandler.HasErrors().Should().BeFalse();

        }

        [Fact(DisplayName = nameof(ValidateWhenInvalidState))]
        [Trait("Domain", "Video - Aggregate")]
        public void ValidateWhenInvalidState()
        {
            var exampleVideo = new DomainEntity.Video(
                   _fixture.GetTooLongTitle(),
                   _fixture.GetTooLongDescription(),
                   _fixture.GetValidYearLauched(),
                   _fixture.GetRandomBoolean(),
                   _fixture.GetRandomBoolean(),
                   _fixture.GetValidVideoDuration(),
                   _fixture.GetRandomRating()
                    );
            var notificationHandler = new NotificationValidationHandler();
            exampleVideo.Validate(notificationHandler);

            notificationHandler.HasErrors().Should().BeTrue();
            notificationHandler.Errors.Should()
                .BeEquivalentTo(new List<ValidationError>()
            {
                 new ValidationError("'Title' should be less or equal 255 characters long"),
                 new ValidationError("'Description' should be less or equal 4000 characters long"),
            });

        }

        [Fact(DisplayName = nameof(Update))]
        [Trait("Domain", "Video - Aggregate")]
        public void Update()
        {
            var title = _fixture.GetValidVideoTitle();
            var description = _fixture.GetValidVideoDescription();
            var year = _fixture.GetValidYearLauched();
            var opened = _fixture.GetRandomBoolean();
            var published = _fixture.GetRandomBoolean();
            var duration = _fixture.GetValidVideoDuration();
            var rating = _fixture.GetRandomRating();
            var video = _fixture.GetValidVideo();

            video.Update(title, description, year, opened, published, duration, rating);

            video.Title.Should().Be(title);
            video.Description.Should().Be(description);
            video.Opened.Should().Be(opened);
            video.Published.Should().Be(published);
            video.YearLauched.Should().Be(year);
            video.Duration.Should().Be(duration);
            video.Rating.Should().Be(rating);
        }


        [Fact(DisplayName = nameof(ValidateAfterUpdate))]
        [Trait("Domain", "Video - Aggregate")]
        public void ValidateAfterUpdate()
        {
            var title = _fixture.GetValidVideoTitle();
            var description = _fixture.GetValidVideoDescription();
            var year = _fixture.GetValidYearLauched();
            var opened = _fixture.GetRandomBoolean();
            var published = _fixture.GetRandomBoolean();
            var duration = _fixture.GetValidVideoDuration();
            var rating = _fixture.GetRandomRating();
            var video = _fixture.GetValidVideo();

            video.Update(title, description, year, opened, published, duration, rating);
            var notificationHandler = new NotificationValidationHandler();
            video.Validate(notificationHandler);

            notificationHandler.HasErrors().Should().BeFalse();
        }

        [Fact(DisplayName = nameof(UpdateInvalid))]
        [Trait("Domain", "Video - Aggregate")]
        public void UpdateInvalid()
        {
            var title = _fixture.GetTooLongTitle();
            var description = _fixture.GetTooLongDescription();
            var year = _fixture.GetValidYearLauched();
            var opened = _fixture.GetRandomBoolean();
            var published = _fixture.GetRandomBoolean();
            var duration = _fixture.GetValidVideoDuration();
            var rating = _fixture.GetRandomRating();
            var video = _fixture.GetValidVideo();

            video.Update(title, description, year, opened, published, duration, rating);
            var notificationHandler = new NotificationValidationHandler();
            video.Validate(notificationHandler);

            notificationHandler.HasErrors().Should().BeTrue();
            notificationHandler.Errors.Should().HaveCount(2);
            notificationHandler.Errors.Should()
                .BeEquivalentTo(new List<ValidationError>()
            {
                 new ValidationError("'Title' should be less or equal 255 characters long"),
                 new ValidationError("'Description' should be less or equal 4000 characters long"),
            });
        }

        [Fact(DisplayName = nameof(UpdateThumb))]
        [Trait("Domain", "Video - Aggregate")]
        public void UpdateThumb()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var validImagePath = _fixture.GetValidImagePath();
            exampleVideo.UpdateThumb(validImagePath);
            exampleVideo.Thumb.Should().NotBeNull();
            exampleVideo.Thumb!.Path.Should().Be(validImagePath);
        }

        [Fact(DisplayName = nameof(UpdateThumbHalf))]
        [Trait("Domain", "Video - Aggregate")]
        public void UpdateThumbHalf()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var validImagePath = _fixture.GetValidImagePath();
            exampleVideo.UpdateThumbHalf(validImagePath);
            exampleVideo.ThumbHalf.Should().NotBeNull();
            exampleVideo.ThumbHalf!.Path.Should().Be(validImagePath);
        }

        [Fact(DisplayName = nameof(UpdateBanner))]
        [Trait("Domain", "Video - Aggregate")]
        public void UpdateBanner()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var validImagePath = _fixture.GetValidImagePath();
            exampleVideo.UpdateBanner(validImagePath);
            exampleVideo.Banner.Should().NotBeNull();
            exampleVideo.Banner!.Path.Should().Be(validImagePath);
        }

        [Fact(DisplayName = nameof(UpdateMidia))]
        [Trait("Domain", "Video - Aggregate")]
        public void UpdateMidia()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var validPath = _fixture.GetValidImagePath();
            exampleVideo.UpdateMedia(validPath);
            exampleVideo.Media.Should().NotBeNull();
            exampleVideo.Media!.FilePath.Should().Be(validPath);
        }

        [Fact(DisplayName = nameof(UpdateTrailer))]
        [Trait("Domain", "Video - Aggregate")]
        public void UpdateTrailer()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var validPath = _fixture.GetValidImagePath();
            exampleVideo.UpdateTrailer(validPath);
            exampleVideo.Trailer.Should().NotBeNull();
            exampleVideo.Trailer!.FilePath.Should().Be(validPath);
        }

        [Fact(DisplayName = nameof(UpdateAsSentToEncode))]
        [Trait("Domain", "Video - Aggregate")]
        public void UpdateAsSentToEncode()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var validPath = _fixture.GetValidImagePath();
            exampleVideo.UpdateMedia(validPath);
            exampleVideo.UpdateAsSentToEncode();
            exampleVideo.Media.Should().NotBeNull();
            exampleVideo.Media!.Status.Should().Be(MediaStatus.Processing);
        }

        [Fact(DisplayName = nameof(ThrowWhenThereIsNoMedia))]
        [Trait("Domain", "Video - Aggregate")]
        public void ThrowWhenThereIsNoMedia()
        {
            var exampleVideo = _fixture.GetValidVideo();

            var action = () => exampleVideo.UpdateAsSentToEncode();
            action.Should().Throw<EntityValidationException>()
                .WithMessage("There is no media");
        }

        [Fact(DisplayName = nameof(UpdateAsEncoded))]
        [Trait("Domain", "Video - Aggregate")]
        public void UpdateAsEncoded()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var validPath = _fixture.GetValidImagePath();
            var validEncodedPath = _fixture.GetValidMediaPath();
            exampleVideo.UpdateMedia(validPath);
            exampleVideo.UpdateAsEncoded(validEncodedPath);
            exampleVideo.Media.Should().NotBeNull();
            exampleVideo.Media!.Status.Should().Be(MediaStatus.Completed);
            exampleVideo.Media!.EncodedPath.Should().Be(validEncodedPath);
        }

        [Fact(DisplayName = nameof(ThrowWhenThereIsNoMediaEncoded))]
        [Trait("Domain", "Video - Aggregate")]
        public void ThrowWhenThereIsNoMediaEncoded()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var validPath = _fixture.GetValidImagePath();
            var action = () => exampleVideo.UpdateAsEncoded(validPath);
            action.Should().Throw<EntityValidationException>()
                .WithMessage("There is no media");
        }

        [Fact(DisplayName = nameof(AddCategory))]
        [Trait("Domain", "Video - Aggregate")]
        public void AddCategory()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var exampleCategoryId = Guid.NewGuid();
            exampleVideo.AddCategory(exampleCategoryId);

            exampleVideo.Categories.Should().HaveCount(1);
            exampleVideo.Categories[0].Should().Be(exampleCategoryId);
        }

        [Fact(DisplayName = nameof(RemoveCategory))]
        [Trait("Domain", "Video - Aggregate")]
        public void RemoveCategory()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var exampleCategoryId = Guid.NewGuid();
            var exampleCategoryId2 = Guid.NewGuid();

            exampleVideo.AddCategory(exampleCategoryId);
            exampleVideo.AddCategory(exampleCategoryId2);

            exampleVideo.RemoveCategory(exampleCategoryId);

            exampleVideo.Categories.Should().HaveCount(1);
            exampleVideo.Categories[0].Should().Be(exampleCategoryId2);
        }

        [Fact(DisplayName = nameof(RemoveAllCategory))]
        [Trait("Domain", "Video - Aggregate")]
        public void RemoveAllCategory()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var exampleCategoryId = Guid.NewGuid();
            var exampleCategoryId2 = Guid.NewGuid();

            exampleVideo.AddCategory(exampleCategoryId);
            exampleVideo.AddCategory(exampleCategoryId2);

            exampleVideo.RemoveAllCategory();

            exampleVideo.Categories.Should().HaveCount(0);
        }

        [Fact(DisplayName = nameof(AddGenre))]
        [Trait("Domain", "Video - Aggregate")]
        public void AddGenre()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var exampleGenreId = Guid.NewGuid();
            exampleVideo.AddGenre(exampleGenreId);

            exampleVideo.Genres.Should().HaveCount(1);
            exampleVideo.Genres[0].Should().Be(exampleGenreId);
        }

        [Fact(DisplayName = nameof(RemoveGenre))]
        [Trait("Domain", "Video - Aggregate")]
        public void RemoveGenre()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var exampleGenreId = Guid.NewGuid();
            var exampleGenreId2 = Guid.NewGuid();

            exampleVideo.AddGenre(exampleGenreId);
            exampleVideo.AddGenre(exampleGenreId2);

            exampleVideo.RemoveGenre(exampleGenreId);

            exampleVideo.Genres.Should().HaveCount(1);
            exampleVideo.Genres[0].Should().Be(exampleGenreId2);
        }

        [Fact(DisplayName = nameof(RemoveAllGenres))]
        [Trait("Domain", "Video - Aggregate")]
        public void RemoveAllGenres()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var exampleGenreId = Guid.NewGuid();
            var exampleGenreId2 = Guid.NewGuid();

            exampleVideo.AddGenre(exampleGenreId);
            exampleVideo.AddGenre(exampleGenreId2);

            exampleVideo.RemoveAllGenres();

            exampleVideo.Genres.Should().HaveCount(0);
        }

        [Fact(DisplayName = nameof(AddCastMember))]
        [Trait("Domain", "Video - Aggregate")]
        public void AddCastMember()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var exampleCastMemberId = Guid.NewGuid();
            exampleVideo.AddCastMember(exampleCastMemberId);

            exampleVideo.CastMembers.Should().HaveCount(1);
            exampleVideo.CastMembers[0].Should().Be(exampleCastMemberId);
        }

        [Fact(DisplayName = nameof(RemoveCastMember))]
        [Trait("Domain", "Video - Aggregate")]
        public void RemoveCastMember()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var exampleCastMemberId = Guid.NewGuid();
            var exampleCastMemberId2 = Guid.NewGuid();

            exampleVideo.AddCastMember(exampleCastMemberId);
            exampleVideo.AddCastMember(exampleCastMemberId2);

            exampleVideo.RemoveCastMember(exampleCastMemberId);

            exampleVideo.CastMembers.Should().HaveCount(1);
            exampleVideo.CastMembers[0].Should().Be(exampleCastMemberId2);
        }

        [Fact(DisplayName = nameof(RemoveAllCastMember))]
        [Trait("Domain", "Video - Aggregate")]
        public void RemoveAllCastMember()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var exampleCastMemberId = Guid.NewGuid();
            var exampleCastMemberId2 = Guid.NewGuid();

            exampleVideo.AddCastMember(exampleCastMemberId);
            exampleVideo.AddCastMember(exampleCastMemberId2);

            exampleVideo.RemoveCastAllMember();

            exampleVideo.CastMembers.Should().HaveCount(0);
        }
    }
}
