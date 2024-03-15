using Xunit;
using FC.Codeflix.Catalog.UniTests.Common.Fixtures;

namespace FC.Codeflix.Catalog.UniTests.Domain.Entity.Video
{
    [CollectionDefinition(nameof(VideoTestFixture))]
    public class VideoTestFixtureCollection : ICollectionFixture<VideoTestFixture>
    { }
    public class VideoTestFixture : VideoBaseTestFixture
    {


    }
}

