using FC.Codeflix.Catalog.UniTests.Common.Fixtures;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.DeleteVideo;

using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Application.Video.DeleteVideo
{
    [CollectionDefinition(nameof(DeleteVideoTestFixture))]
    public class DeleteVideoTestFixtureCollection : ICollectionFixture<DeleteVideoTestFixture>
    { }

    public class DeleteVideoTestFixture : VideoBaseTestFixture
    {
        public UseCase.DeleteVideoInput GetValidInput(Guid? Id = null)
           => new(
               Id ?? Guid.NewGuid()
            );
    }
}
