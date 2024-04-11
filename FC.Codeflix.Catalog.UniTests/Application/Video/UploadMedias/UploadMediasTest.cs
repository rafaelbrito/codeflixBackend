using FC.Codeflix.Catalog.Application.Inferfaces;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.UploadMedias;
using FC.Codeflix.Catalog.Domain.Repository;
using Moq;
using Xunit;
using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.Exceptions;
using FluentAssertions;

namespace FC.Codeflix.Catalog.UniTests.Application.Video.UploadMedias
{
    [Collection(nameof(UploadMediasTestFixture))]
    public class UploadMediasTest
    {
        private readonly UploadMediasTestFixture _fixture;
        private readonly Mock<IVideoRepository> _videoRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IStorageService> _storageServiceMock;
        private readonly UseCase.UploadMedias _useCase;
        public UploadMediasTest(UploadMediasTestFixture fixture)
        {
            _fixture = fixture;
            _videoRepositoryMock = new Mock<IVideoRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _storageServiceMock = new Mock<IStorageService>();
            _useCase = new UseCase.UploadMedias(
                _videoRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _storageServiceMock.Object
                );
        }



        [Fact(DisplayName = (nameof(UploadMedias)))]
        [Trait("Application", "UploadMedias - Use Cases")]
        public async Task UploadMedias()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var input = _fixture.GetValidInput(exampleVideo.Id);
            var fileName = new List<string>()
            {
                StorageFileName.Create(exampleVideo.Id, nameof(exampleVideo.Media), input.VideoFile!.Extension),
                StorageFileName.Create(exampleVideo.Id, nameof(exampleVideo.Trailer), input.TraileFile!.Extension)
            };

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == exampleVideo.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleVideo);

            _storageServiceMock.Setup(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(Guid.NewGuid().ToString());

            await _useCase.Handle(input, CancellationToken.None);

            _videoRepositoryMock.VerifyAll();

            _storageServiceMock.Verify(x => x.Upload(
                It.Is<string>(x => fileName.Contains(x)),
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
                Times.Exactly(2));

            _unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()));
        }

        [Fact(DisplayName = (nameof(ThrowsWhenVideoNotFound)))]
        [Trait("Application", "UploadMedias - Use Cases")]
        public async Task ThrowsWhenVideoNotFound()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var input = _fixture.GetValidInput(exampleVideo.Id);

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == exampleVideo.Id),
                It.IsAny<CancellationToken>()
                )).ThrowsAsync(new NotFoundException("Video not found"));

            var action = () => _useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Video not found");
        }

        [Fact(DisplayName = (nameof(ClearStorageInUploadErrorCase)))]
        [Trait("Application", "UploadMedias - Use Cases")]
        public async Task ClearStorageInUploadErrorCase()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var input = _fixture.GetValidInput(exampleVideo.Id);
            var videoFileName = StorageFileName.Create(exampleVideo.Id, nameof(exampleVideo.Media), input.VideoFile!.Extension);
            var trailerFileName = StorageFileName.Create(exampleVideo.Id, nameof(exampleVideo.Trailer), input.TraileFile!.Extension);
            var videoStoragePath = $"storage/{videoFileName}";
            var trailerStoragePath = $"storage/{trailerFileName}";

            var fileName = new List<string>() { videoFileName, trailerFileName };

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == exampleVideo.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleVideo);

            _storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(x => x == videoFileName),
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(videoStoragePath);


            _storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(x => x == trailerFileName),
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
                )).ThrowsAsync(new Exception("Something went wrong with the upload"));

            var action = () => _useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<Exception>()
                .WithMessage("Something went wrong with the upload");

            _videoRepositoryMock.VerifyAll();

            _storageServiceMock.Verify(x => x.Upload(
                It.Is<string>(x => fileName.Contains(x)),
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
                Times.Exactly(2));

            _storageServiceMock.Verify(x => x.Delete(
                It.Is<string>(fileName => fileName == videoStoragePath),
                It.IsAny<CancellationToken>()
                ), Times.Once);
        }

        [Fact(DisplayName = (nameof(ClearStorageInCommitErrorCase)))]
        [Trait("Application", "UploadMedias - Use Cases")]
        public async Task ClearStorageInCommitErrorCase()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var input = _fixture.GetValidInput(exampleVideo.Id);
            var videoFileName = StorageFileName.Create(exampleVideo.Id, nameof(exampleVideo.Media), input.VideoFile!.Extension);
            var trailerFileName = StorageFileName.Create(exampleVideo.Id, nameof(exampleVideo.Trailer), input.TraileFile!.Extension);
            var videoStoragePath = $"storage/{videoFileName}";
            var trailerStoragePath = $"storage/{trailerFileName}";

            var fileName = new List<string>() { videoFileName, trailerFileName };
            var filePathName = new List<string>() { videoStoragePath, trailerStoragePath };


            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == exampleVideo.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleVideo);

            _storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(x => x == videoFileName),
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(videoStoragePath);

            _storageServiceMock.Setup(x => x.Upload(
               It.Is<string>(x => x == trailerFileName),
               It.IsAny<Stream>(),
               It.IsAny<string>(),
               It.IsAny<CancellationToken>()
               )).ReturnsAsync(trailerStoragePath);

            _unitOfWorkMock.Setup(x => x.Commit(
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Something went wrong with the commit")
                );


            var action = () => _useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<Exception>()
                .WithMessage("Something went wrong with the commit");

            _videoRepositoryMock.VerifyAll();

            _storageServiceMock.Verify(x => x.Upload(
                It.Is<string>(x => fileName.Contains(x)),
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
                Times.Exactly(2));

            _storageServiceMock.Verify(x => x.Delete(
                It.Is<string>(fileName => filePathName.Contains(fileName)),
                It.IsAny<CancellationToken>()
                ), Times.Exactly(2));

            _storageServiceMock.Verify(x => x.Delete(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
                ), Times.Exactly(2));
        }

        [Fact(DisplayName = (nameof(ClearOnlyOneFileInStorageInCommitErrorCase)))]
        [Trait("Application", "UploadMedias - Use Cases")]
        public async Task ClearOnlyOneFileInStorageInCommitErrorCase()
        {
            var exampleVideo = _fixture.GetValidVideo();
            exampleVideo.UpdateTrailer(_fixture.GetValidMediaPath());
            exampleVideo.UpdateMedia(_fixture.GetValidMediaPath());
            var input = _fixture.GetValidInput(exampleVideo.Id, withTrailerFile: false);
            var videoFileName = StorageFileName.Create(exampleVideo.Id, nameof(exampleVideo.Media), input.VideoFile!.Extension);
            var videoStoragePath = $"storage/{videoFileName}";

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == exampleVideo.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleVideo);

            _storageServiceMock.Setup(x => x.Upload(
               It.IsAny<string>(),
               It.IsAny<Stream>(),
               It.IsAny<string>(),
               It.IsAny<CancellationToken>()
               )).ReturnsAsync(Guid.NewGuid().ToString());

            _storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(x => x == videoFileName),
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(videoStoragePath);

            _unitOfWorkMock.Setup(x => x.Commit(
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Something went wrong with the commit")
                );


            var action = () => _useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<Exception>()
                .WithMessage("Something went wrong with the commit");

            _videoRepositoryMock.VerifyAll();

            _storageServiceMock.Verify(x => x.Upload(
                It.Is<string>(x => x == videoFileName),
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
                Times.Once);

            _storageServiceMock.Verify(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
                Times.Once);

            _storageServiceMock.Verify(x => x.Delete(
                It.Is<string>(fileName => fileName == videoStoragePath),
                It.IsAny<CancellationToken>()
                ), Times.Once);

            _storageServiceMock.Verify(x => x.Delete(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
                ), Times.Once);
        }
    }
}
