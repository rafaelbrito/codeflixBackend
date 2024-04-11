using GcpData = Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using System.Text;
using FC.Codeflix.Catalog.Infra.Storage.Configuration;
using FC.Codeflix.Catalog.Infra.Storage.Services;
using Google.Apis.Upload;


namespace FC.Codeflix.Catalog.UniTests.Infra.Storage
{
    [Collection(nameof(StorageServiceTestFixture))]
    public class StorageServiceTest
    {
        private readonly StorageServiceTestFixture _fixture;
        public StorageServiceTest(StorageServiceTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(Upload))]
        [Trait("Infra.Storage", "StorageService")]
        public async Task Upload()
        {
            var storageClienteMock = new Mock<StorageClient>();
            var objetcMock = new Mock<GcpData.Object>();

            storageClienteMock.Setup(x => x.UploadObjectAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<UploadObjectOptions>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<IProgress<IUploadProgress>>()))
            .ReturnsAsync(objetcMock.Object);

            var storageOptions = new StorageServiceOptions
            {
                BucketName = _fixture.GetBucketName()
            };
            var options = Options.Create(storageOptions);
            var service = new StorageService(storageClienteMock.Object, options);

            var fileName = _fixture.GetFileName();
            var contentStream = Encoding.UTF8.GetBytes(_fixture.GetContentFile());
            var stream = new MemoryStream(contentStream);
            var contentType = _fixture.GetContentType();
            var filePath = await service.Upload(fileName, stream, contentType, CancellationToken.None);

            Assert.Equal(fileName, filePath);

            storageClienteMock.Verify(x => x.UploadObjectAsync(
                 storageOptions.BucketName,
                 fileName,
                 contentType,
                 stream,
                 It.IsAny<UploadObjectOptions>(),
                 It.IsAny<CancellationToken>(),
                 It.IsAny<IProgress<IUploadProgress>>()
                ), Times.Once);

        }

        [Fact(DisplayName = nameof(Delete))]
        [Trait("Infra.Storage", "StorageService")]
        public async Task Delete()
        {
            var storageClienteMock = new Mock<StorageClient>();
            var objetcMock = new Mock<GcpData.Object>();

            storageClienteMock.Setup(x => x.DeleteObjectAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DeleteObjectOptions>(),
                It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

            var storageOptions = new StorageServiceOptions
            {
                BucketName = _fixture.GetBucketName()
            };
            var options = Options.Create(storageOptions);
            var service = new StorageService(storageClienteMock.Object, options);

            var fileName = _fixture.GetFileName();

            await service.Delete(fileName, CancellationToken.None);

            storageClienteMock.Verify(x => x.DeleteObjectAsync(
                 storageOptions.BucketName,
                 fileName,
                 It.IsAny<DeleteObjectOptions>(),
                 It.IsAny<CancellationToken>()
                ), Times.Once);
        }
    }
}
