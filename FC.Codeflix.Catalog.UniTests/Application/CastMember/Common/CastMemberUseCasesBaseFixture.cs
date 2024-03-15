using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.UniTests.Common.Fixtures;

namespace FC.Codeflix.Catalog.UniTests.Application.UseCases.CastMember.Common
{
    public class CastMemberUseCasesBaseFixture : BaseFixture
    {
        public string GetValidName()
           => Faker.Name.FullName();

        public CastMemberType GetRandomCastMemberType()
            => (CastMemberType)(new Random().Next(1, 2));

        public DomainEntity.CastMember GetExampleCastMember()
            => new(GetValidName(), GetRandomCastMemberType());

        public List<DomainEntity.CastMember> GetExampleCastMemberList(int length = 10)
         => Enumerable.Range(0, length)
            .Select(_ =>
            GetExampleCastMember())
            .ToList();
    }
}
