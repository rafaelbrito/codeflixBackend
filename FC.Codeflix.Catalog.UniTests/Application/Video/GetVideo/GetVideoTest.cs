using FC.Codeflix.Catalog.Application.Inferfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.GetVideo;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Moq;
using Xunit;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Extensions;

namespace FC.Codeflix.Catalog.UniTests.Application.Video.GetVideo
{
    [Collection(nameof(GetVideoTestFixture))]
    public class GetVideoTest
    {
        private readonly GetVideoTestFixture _fixture;
        private readonly Mock<IVideoRepository> _videoRepositoryMock;
        private readonly UseCase.GetVideo _useCase;

        public GetVideoTest(GetVideoTestFixture fixture)
        {
            _fixture = fixture;
            _videoRepositoryMock = new Mock<IVideoRepository>();
            _useCase = new UseCase.GetVideo(
                _videoRepositoryMock.Object);
        }

        [Fact(DisplayName = nameof(Get))]
        [Trait("Application", "GetVideo - Use Cases")]
        public async Task Get()
        {
            var videoExample = _fixture.GetValidVideo();

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(id => id == videoExample.Id),
                It.IsAny<CancellationToken>()
             )).ReturnsAsync(videoExample);

            var input = new UseCase.GetVideoInput(videoExample.Id);

            var output = await _useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(videoExample.Id);
            output.Title.Should().Be(videoExample.Title);
            output.Description.Should().Be(videoExample.Description);
            output.YearLauched.Should().Be(videoExample.YearLauched);
            output.Opened.Should().Be(videoExample.Opened);
            output.Published.Should().Be(videoExample.Published);
            output.Duration.Should().Be(videoExample.Duration);
            output.Rating.Should().Be(videoExample.Rating.ToStringSignal());

            _videoRepositoryMock.VerifyAll();


        }

        [Fact(DisplayName = nameof(GetWithAllProperties))]
        [Trait("Application", "GetVideo - Use Cases")]
        public async Task GetWithAllProperties()
        {
            var videoExample = _fixture.GetValidVideoWithAllProperties();

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(id => id == videoExample.Id),
                It.IsAny<CancellationToken>()
             )).ReturnsAsync(videoExample);

            var input = new UseCase.GetVideoInput(videoExample.Id);

            var output = await _useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(videoExample.Id);
            output.Title.Should().Be(videoExample.Title);
            output.Description.Should().Be(videoExample.Description);
            output.YearLauched.Should().Be(videoExample.YearLauched);
            output.Opened.Should().Be(videoExample.Opened);
            output.Published.Should().Be(videoExample.Published);
            output.Duration.Should().Be(videoExample.Duration);
            output.Rating.Should().Be(videoExample.Rating.ToStringSignal());
            output.ThumbFileUrl.Should().Be(videoExample.Thumb!.Path);
            output.ThumbHalfFileUrl.Should().Be(videoExample.Thumb.Path);
            output.BannerFileUrl.Should().Be(videoExample.Banner!.Path);
            output.VideoFileUrl.Should().Be(videoExample.Media!.FilePath);
            output.TrailerFileUrl.Should().Be(videoExample.Trailer!.FilePath);


            var outputItemCategories = output.Categories
                    .Select(dto => dto.Id).ToList();
            outputItemCategories.Should().BeEquivalentTo(videoExample.Categories);

            var outputItemGenres = output.Genres
                .Select(dto => dto.Id).ToList();
            outputItemGenres.Should().BeEquivalentTo(videoExample.Genres);

            var outputItemCastMembers = output.CastMembers
                .Select(dto => dto.Id).ToList();
            outputItemCastMembers.Should().BeEquivalentTo(videoExample.CastMembers);

            _videoRepositoryMock.VerifyAll();
        }

        [Fact(DisplayName = nameof(ThrowExceptionWhenNotFound))]
        [Trait("Application", "GetVideo - Use Cases")]
        public async Task ThrowExceptionWhenNotFound()
        {
            _videoRepositoryMock.Setup(x => x.Get(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
             )).ThrowsAsync(new NotFoundException("Video not found"));

            var input = new UseCase.GetVideoInput(Guid.NewGuid());
            var action = () => _useCase.Handle(input, CancellationToken.None);
            await action.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Video not found");
            _videoRepositoryMock.VerifyAll();
        }
    }
}