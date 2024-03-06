using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.Common;
using FluentAssertions;
using Xunit;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Application.Exceptions;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.UpdateCastMember
{
    [Collection(nameof(CastMemberUseCaseBaseFixture))]
    public class UpdateCastMemberTest
    {
        private readonly CastMemberUseCaseBaseFixture _fixture;
        public UpdateCastMemberTest(CastMemberUseCaseBaseFixture fixture)
            => _fixture = fixture;


        [Fact(DisplayName = (nameof(Update)))]
        [Trait("Integration/Application", "UpdateCastMember - Use Cases")]
        public async Task Update()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleCastMember = _fixture.GetExampleCastMemberList();
            var targetCastMember = exampleCastMember[5];
            var newName = _fixture.GetValidName();
            var newType = _fixture.GetRandomCastMemberType();
            await dbContext.CastMembers.AddRangeAsync(exampleCastMember, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var actDbContext = _fixture.CreateDbContext(true);
            var repository = new CastMemberRepository(actDbContext);
            var unitOfWork = new UnitOfWork(actDbContext);


            var useCase = new UseCase.UpdateCastMember(repository, unitOfWork);
            var input = new UseCase.UpdateCastMemberInput(targetCastMember.Id, newName, newType);
            var output = await useCase.Handle(input, CancellationToken.None);

            output.Id.Should().Be(targetCastMember.Id);
            output.Name.Should().Be(newName);
            output.Type.Should().Be(newType);
            var assertDbContext = _fixture.CreateDbContext(true);
            var castMember = assertDbContext.CastMembers
                .FirstOrDefault(x => x.Id == output.Id);
            castMember.Should().NotBeNull();
            castMember!.Name.Should().Be(newName);
            castMember.Type.Should().Be(newType);
        }

        [Fact(DisplayName = (nameof(ThrowWhenNotFound)))]
        [Trait("Integration/Application", "UpdateCastMember - Use Cases")]
        public async Task ThrowWhenNotFound()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleCastMember = _fixture.GetExampleCastMemberList();
            var randomGuid = Guid.NewGuid();
            await dbContext.CastMembers.AddRangeAsync(exampleCastMember, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var actDbContext = _fixture.CreateDbContext(true);
            var repository = new CastMemberRepository(actDbContext);
            var unitOfWork = new UnitOfWork(actDbContext);


            var useCase = new UseCase.UpdateCastMember(repository, unitOfWork);
            var input = new UseCase.UpdateCastMemberInput(randomGuid, _fixture.GetValidName(), _fixture.GetRandomCastMemberType());
            var action = async () => await useCase.Handle(input, CancellationToken.None);
          
            await action.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"CastMember '{input.Id}' not found.");
        }
    }
}
