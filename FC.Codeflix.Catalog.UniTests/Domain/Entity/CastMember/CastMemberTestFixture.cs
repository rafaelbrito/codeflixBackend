using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.UniTests.Comon;
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
        public string GetValidName()
            => Faker.Name.FullName();

        public CastMemberType GetRandomCastMemberType()
            => (CastMemberType)(new Random().Next(1, 2));

        public DomainEntity.CastMember GetExampleCastMember()
            => new(GetValidName(), GetRandomCastMemberType());
    }
}

