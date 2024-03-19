using FC.Codeflix.Catalog.Application.Inferfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.DeleteVideo;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Moq;
using Xunit;
using FC.Codeflix.Catalog.Application.Exceptions;
using FluentAssertions;

namespace FC.Codeflix.Catalog.UniTests.Application.Video.DeleteVideo
{
    [Collection(nameof(DeleteVideoTestFixture))]
    public class DeleteVideoTest
    {
        private readonly DeleteVideoTestFixture _fixture;
        private readonly Mock<IVideoRepository> _videoRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IStorageService> _storageServiceMock;
        private readonly UseCase.DeleteVideo _useCase;

        public DeleteVideoTest(DeleteVideoTestFixture fixture)
        {
            _fixture = fixture;
            _videoRepositoryMock = new Mock<IVideoRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _storageServiceMock = new Mock<IStorageService>();
            _useCase = new UseCase.DeleteVideo(
                _videoRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _storageServiceMock.Object);
        }

        [Fact(DisplayName = nameof(DeleteVideo))]
        [Trait("Apllication", "DeleteVideo - Use Cases")]
        public async Task DeleteVideo()
        {
            var videoExample = _fixture.GetValidVideo();
            var input = _fixture.GetValidInput(videoExample.Id);

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == videoExample.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(videoExample);

            await _useCase.Handle(input, CancellationToken.None);

            _videoRepositoryMock.VerifyAll();

            _videoRepositoryMock.Verify(x => x.Delete(
                It.Is<DomainEntity.Video>(video => video.Id == videoExample.Id),
                It.IsAny<CancellationToken>()
                ), Times.Once);

            _unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()));
        }

        [Fact(DisplayName = nameof(DeleteVideoWithAllMedias))]
        [Trait("Apllication", "DeleteVideo - Use Cases")]
        public async Task DeleteVideoWithAllMedias()
        {
            var videoExample = _fixture.GetValidVideo();
            videoExample.UpdateMedia(_fixture.GetValidMediaPath());
            videoExample.UpdateTrailer(_fixture.GetValidMediaPath());
            var filePath = new List<string> {
                videoExample.Media!.FilePath,
                videoExample.Trailer!.FilePath
            };
            var input = _fixture.GetValidInput(videoExample.Id);

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == videoExample.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(videoExample);

            await _useCase.Handle(input, CancellationToken.None);

            _videoRepositoryMock.VerifyAll();

            _videoRepositoryMock.Verify(x => x.Delete(
                It.Is<DomainEntity.Video>(video => video.Id == videoExample.Id),
                It.IsAny<CancellationToken>()
                ), Times.Once);

            _unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()));

            _storageServiceMock.Verify(x => x.Delete(
                It.Is<string>(filePath => filePath.Contains(filePath)),
                It.IsAny<CancellationToken>()),
                Times.Exactly(2));

            _storageServiceMock.Verify(x => x.Delete(
             It.IsAny<string>(),
             It.IsAny<CancellationToken>()),
             Times.Exactly(2));
        }

        [Fact(DisplayName = nameof(DeleteVideoWithTrailer))]
        [Trait("Apllication", "DeleteVideo - Use Cases")]
        public async Task DeleteVideoWithTrailer()
        {
            var videoExample = _fixture.GetValidVideo();
            videoExample.UpdateTrailer(_fixture.GetValidMediaPath());

            var input = _fixture.GetValidInput(videoExample.Id);

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == videoExample.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(videoExample);

            await _useCase.Handle(input, CancellationToken.None);

            _videoRepositoryMock.VerifyAll();

            _videoRepositoryMock.Verify(x => x.Delete(
                It.Is<DomainEntity.Video>(video => video.Id == videoExample.Id),
                It.IsAny<CancellationToken>()
                ), Times.Once);

            _unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()));

            _storageServiceMock.Verify(x => x.Delete(
                It.Is<string>(filePath => filePath == videoExample.Trailer!.FilePath),
                It.IsAny<CancellationToken>()),
                Times.Once);

            _storageServiceMock.Verify(x => x.Delete(
             It.IsAny<string>(),
             It.IsAny<CancellationToken>()),
             Times.Once);
        }

        [Fact(DisplayName = nameof(DeleteVideoWithMedia))]
        [Trait("Apllication", "DeleteVideo - Use Cases")]
        public async Task DeleteVideoWithMedia()
        {
            var videoExample = _fixture.GetValidVideo();
            videoExample.UpdateMedia(_fixture.GetValidMediaPath());

            var input = _fixture.GetValidInput(videoExample.Id);

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == videoExample.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(videoExample);

            await _useCase.Handle(input, CancellationToken.None);

            _videoRepositoryMock.VerifyAll();

            _videoRepositoryMock.Verify(x => x.Delete(
                It.Is<DomainEntity.Video>(video => video.Id == videoExample.Id),
                It.IsAny<CancellationToken>()
                ), Times.Once);

            _unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()));

            _storageServiceMock.Verify(x => x.Delete(
                It.Is<string>(filePath => filePath == videoExample.Media!.FilePath),
                It.IsAny<CancellationToken>()),
                Times.Once);

            _storageServiceMock.Verify(x => x.Delete(
             It.IsAny<string>(),
             It.IsAny<CancellationToken>()),
             Times.Once);
        }

        [Fact(DisplayName = nameof(DeleteVideoDontMedias))]
        [Trait("Apllication", "DeleteVideo - Use Cases")]
        public async Task DeleteVideoDontMedias()
        {
            var videoExample = _fixture.GetValidVideo();
            var input = _fixture.GetValidInput(videoExample.Id);

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == videoExample.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(videoExample);

            await _useCase.Handle(input, CancellationToken.None);

            _videoRepositoryMock.VerifyAll();

            _videoRepositoryMock.Verify(x => x.Delete(
                It.Is<DomainEntity.Video>(video => video.Id == videoExample.Id),
                It.IsAny<CancellationToken>()
                ), Times.Once);

            _unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()));

            _storageServiceMock.Verify(x => x.Delete(
             It.IsAny<string>(),
             It.IsAny<CancellationToken>()),
             Times.Never);
        }

        [Fact(DisplayName = nameof(ThrowVideoNotFoundException))]
        [Trait("Apllication", "DeleteVideo - Use Cases")]
        public async Task ThrowVideoNotFoundException()
        {
            var input = _fixture.GetValidInput();

            _videoRepositoryMock.Setup(x => x.Get(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
                )).ThrowsAsync(new NotFoundException("Video not found"));

            var action = () => _useCase.Handle(input, CancellationToken.None);
            await action.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Video not found");

            _videoRepositoryMock.VerifyAll();

            _videoRepositoryMock.Verify(x => x.Delete(
                It.IsAny<DomainEntity.Video>(),
                It.IsAny<CancellationToken>()
                ), Times.Never);

            _unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()), Times.Never);

            _storageServiceMock.Verify(x => x.Delete(
             It.IsAny<string>(),
             It.IsAny<CancellationToken>()),
             Times.Never);
        }
    }
}
