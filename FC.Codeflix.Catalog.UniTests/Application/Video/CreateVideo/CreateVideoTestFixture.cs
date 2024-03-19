using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using FC.Codeflix.Catalog.UniTests.Common.Fixtures;
using System.Text;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Application.Video.CreateVideo
{
    [CollectionDefinition(nameof(CreateVideoTestFixture))]
    public class CreateVideoTestFixtureCollection : ICollectionFixture<CreateVideoTestFixture>
    { }
    public class CreateVideoTestFixture : VideoBaseTestFixture
    {
        internal CreateVideoInput CreateValidInputAllMedias()
        => new(
               GetValidVideoTitle(),
               GetValidVideoDescription(),
               GetValidYearLauched(),
               GetRandomBoolean(),
               GetRandomBoolean(),
               GetValidVideoDuration(),
               GetRandomRating(),
               null,
               null,
               null,
               null,
               null,
               null,
               GetValidMediaFileInput(),
               GetValidMediaFileInput()
               );
    }
}
