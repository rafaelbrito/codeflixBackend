using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CastMemberRepository;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.GetCastMember;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.Exceptions;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.GetCastMember
{
    [Collection(nameof(CastMemberRepositoryTestFixture))]
    public class GetCastMemberTest
    {
        private readonly CastMemberRepositoryTestFixture _fixture;

        public GetCastMemberTest(CastMemberRepositoryTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(Get))]
        [Trait("Integration/Application", "GetCastMember - Use Cases")]
        public async void Get()
        {
            var dbContext = _fixture.CreateDbContext();
            var exapleCastMemberList = _fixture.GetExampleCastMemberList();
            var targetCastMember = exapleCastMemberList[5];
            await dbContext.AddRangeAsync(exapleCastMemberList, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var repository = new CastMemberRepository(dbContext);
            var useCase = new UseCase.GetCastMember(repository);
            var input = new UseCase.GetCastMemberInput(targetCastMember.Id);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output!.Id.Should().NotBeEmpty();
            output!.Name.Should().Be(targetCastMember.Name);
            output.Type.Should().Be(targetCastMember.Type);
            output.CreatedAt.Should().Be(targetCastMember.CreatedAt);
        }

        [Fact(DisplayName = nameof(ThrowWhenNotFound))]
        [Trait("Integration/Application", "GetCastMember - Use Cases")]
        public async void ThrowWhenNotFound()
        {
            var dbContext = _fixture.CreateDbContext();
            var exapleCastMemberList = _fixture.GetExampleCastMemberList();
            var randomGuid = Guid.NewGuid();
            await dbContext.AddRangeAsync(exapleCastMemberList, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var repository = new CastMemberRepository(dbContext);
            var useCase = new UseCase.GetCastMember(repository);
            var input = new UseCase.GetCastMemberInput(randomGuid);

            var action = async () => await useCase.Handle(input, CancellationToken.None);
            await action.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"CastMember '{input.Id}' not found.");
        }
    }
}
