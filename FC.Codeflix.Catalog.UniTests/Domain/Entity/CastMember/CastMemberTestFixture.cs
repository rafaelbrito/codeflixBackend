using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using Xunit;
using FC.Codeflix.Catalog.UniTests.Application.UseCases.CastMember.Common;

namespace FC.Codeflix.Catalog.UniTests.Domain.Entity.CastMember
{
    [CollectionDefinition(nameof(CastMemberTestFixture))]
    public class CastMemberTestFixtureCollection : ICollectionFixture<CastMemberTestFixture>
    {
    }
    public class CastMemberTestFixture : CastMemberUseCasesBaseFixture
    {
    }
}

