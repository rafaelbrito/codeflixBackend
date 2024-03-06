using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.Common;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.CreateCastMember
{
    [CollectionDefinition(nameof (CreateCastMemberTestFixture))]
    public class CreateCastMemberTestFixtureCollection : ICollectionFixture<CreateCastMemberTestFixture>
    { }
    public class CreateCastMemberTestFixture:CastMemberUseCaseBaseFixture
    {
        public CreateCastMemberInput GetExampleInput()
        {
            var castMember = GetExampleCastMember();
            return new CreateCastMemberInput(castMember.Name, castMember.Type);
        }
    }
}
