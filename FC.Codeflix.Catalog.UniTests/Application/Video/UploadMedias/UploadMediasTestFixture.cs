using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.UploadMedias;
using FC.Codeflix.Catalog.UniTests.Common.Fixtures;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Application.Video.UploadMedias
{
    [CollectionDefinition(nameof(UploadMediasTestFixture))]
    public class UploadMediasTestFixtureCollection : ICollectionFixture<UploadMediasTestFixture>
    { }
    public class UploadMediasTestFixture : VideoBaseTestFixture
    {
        public UseCase.UploadMediasInput GetValidInput(Guid? Id = null, bool withVideoFile = true, bool withTrailerFile = true)
            => new(
                Id ?? Guid.NewGuid(),
                withVideoFile ? GetValidMediaFileInput() : null,
                withTrailerFile ? GetValidMediaFileInput() : null
             );
    }
}
