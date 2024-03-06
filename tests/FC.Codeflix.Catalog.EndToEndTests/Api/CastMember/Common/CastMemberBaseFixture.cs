using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;
using FC.Codeflix.Catalog.EndToEndTests.Base;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common
{
    [CollectionDefinition(nameof(CastMemberBaseFixture))]
    public class CastMemberUseCaseBastFixtureCollection : ICollectionFixture<CastMemberBaseFixture>
    { }
}
public class CastMemberBaseFixture : BaseFixture
{
    public CastMemberPersistence Persistence;

    public CastMemberBaseFixture()
        : base()
    {

        Persistence = new CastMemberPersistence(CreateDbContext());
    }

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

    public List<DomainEntity.CastMember> GetExampleCastMemberListWithNames(List<string> names)
       => names.Select(name =>
       {
           var castMember = GetExampleCastMember();
           castMember.Update(name, castMember.Type);
           return castMember;
       }).ToList();

    public List<DomainEntity.CastMember> CloneCastMembersListOrdered(
        List<DomainEntity.CastMember> castMemberList, string orderBy, SearchOrder order)
    {
        var query = new List<DomainEntity.CastMember>(castMemberList);
        var orderedEnumerable = (orderBy.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => query.OrderBy(x => x.Name)
                     .ThenBy(x => x.Id),
            ("name", SearchOrder.Desc) => query.OrderByDescending(x => x.Name)
                     .ThenByDescending(x => x.Id),
            ("type", SearchOrder.Asc) => query.OrderBy(x => x.Type)
           .ThenBy(x => x.Type),
            ("type", SearchOrder.Desc) => query.OrderByDescending(x => x.Type)
                     .ThenByDescending(x => x.Type),
            ("id", SearchOrder.Asc) => query.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => query.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => query.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderBy(x => x.Name)
                    .ThenBy(x => x.Id),
        };
        return orderedEnumerable.ToList();
    }
}

