using FC.Codeflix.Catalog.UniTests.Application.UseCases.CastMember.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Application.UseCases.CastMember.GetCastMember
{
    [CollectionDefinition(nameof(GetCastMemberTestFixture))]
    public class GetCastMemberTestFixtureCollection : ICollectionFixture<GetCastMemberTestFixture>
    { }
    public class GetCastMemberTestFixture: CastMemberUseCasesBaseFixture
    {
    }
}
