using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.DeleteCastMember;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.Common;
using Xunit;
using FC.Codeflix.Catalog.Application.Inferfaces;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Bogus;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.DeleteCastMember
{
    [Collection(nameof(CastMemberUseCaseBaseFixture))]
    public class DeleteCastMemberTest
    {
        private readonly CastMemberUseCaseBaseFixture _fixture;
        public DeleteCastMemberTest(CastMemberUseCaseBaseFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(Delete))]
        [Trait("Integration/Application", "DeleteCastMember - Use Cases")]
        public async void Delete()
        {
            var dbContext = _fixture.CreateDbContext();
            var exapleCastMemberList = _fixture.GetExampleCastMemberList();
            var targetCastMember = exapleCastMemberList[5];
            await dbContext.AddRangeAsync(exapleCastMemberList, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var actDbContext = _fixture.CreateDbContext(true);
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var eventPublisher = new DomainEventPublisher(serviceProvider);
            var unitOfWork = new UnitOfWork(
                actDbContext,
                eventPublisher,
                serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());
            var repository = new CastMemberRepository(actDbContext);
            var useCase = new UseCase.DeleteCastMember(repository, unitOfWork);
            var input = new UseCase.DeleteCastMemberInput(targetCastMember.Id);

            await useCase.Handle(input, CancellationToken.None);
            var assertDb = _fixture.CreateDbContext(true);
            var castMember = await assertDb.CastMembers.AsNoTracking().ToListAsync();
            var returnCastMember = await assertDb.CastMembers.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == targetCastMember.Id);
            returnCastMember.Should().BeNull();
            castMember.Should().HaveCount(9);
        }

        [Fact(DisplayName = nameof(ThrowWhenNotFound))]
        [Trait("Integration/Application", "DeleteCastMember - Use Cases")]
        public async void ThrowWhenNotFound()
        {
            var randomGuid = Guid.NewGuid();
            var actDbContext = _fixture.CreateDbContext();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var eventPublisher = new DomainEventPublisher(serviceProvider);
            var unitOfWork = new UnitOfWork(
                actDbContext,
                eventPublisher,
                serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());

            var repository = new CastMemberRepository(actDbContext);
            var useCase = new UseCase.DeleteCastMember(repository, unitOfWork);
            var input = new UseCase.DeleteCastMemberInput(randomGuid);

            var action = async () => await useCase.Handle(input, CancellationToken.None);
            await action.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"CastMember '{randomGuid}' not found.");
        }

    }
}
