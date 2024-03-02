using FC.Codeflix.Catalog.UniTests.Application.UseCases.CastMember.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Application.UseCases.CastMember.CreateCastMember
{
    [CollectionDefinition(nameof(CreateCastMemberTestFixture))]
    public class CreateCastMemberTestFixtureCollection: ICollectionFixture<CreateCastMemberTestFixture>
    { }
    public class CreateCastMemberTestFixture :CastMemberUseCasesBaseFixture
    {
    }
}
