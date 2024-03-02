using FC.Codeflix.Catalog.UniTests.Application.UseCases.CastMember.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Application.UseCases.CastMember.DeleteCastMember
{
    [CollectionDefinition(nameof(DeleteCastMemberTestFixture))]
    public class DeleteCastMemberTestFixtureCollection : ICollectionFixture<DeleteCastMemberTestFixture>
    { }

    public class DeleteCastMemberTestFixture : CastMemberUseCasesBaseFixture
    {
    }
}
