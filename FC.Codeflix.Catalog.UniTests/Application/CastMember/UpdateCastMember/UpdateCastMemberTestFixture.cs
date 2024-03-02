using FC.Codeflix.Catalog.UniTests.Application.UseCases.CastMember.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Application.UseCases.CastMember.UpdateCastMember
{
    [CollectionDefinition(nameof(UpdateCastMemberTestFixture))]
    public class UpdateCastMemberTestCollection : ICollectionFixture<UpdateCastMemberTestFixture>
    { }

    public class UpdateCastMemberTestFixture:CastMemberUseCasesBaseFixture
    {
    }
}
