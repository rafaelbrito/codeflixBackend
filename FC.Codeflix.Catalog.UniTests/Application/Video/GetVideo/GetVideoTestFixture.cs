using FC.Codeflix.Catalog.UniTests.Common.Fixtures;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Application.Video.GetVideo
{
    [CollectionDefinition(nameof(GetVideoTestFixture))]
    public class GetVideoTestFixtureCollection : ICollectionFixture<GetVideoTestFixture>
    { }

    public class GetVideoTestFixture : VideoBaseTestFixture
    {
    }
}
