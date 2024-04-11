using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using Xunit;
using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using FC.Codeflix.Catalog.Application;
using Microsoft.Extensions.Logging;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.CreateCastMember
{
    [Collection(nameof(CreateCastMemberTestFixture))]
    public class CreateCastMemberTest
    {
        private readonly CreateCastMemberTestFixture _fixture;
        public CreateCastMemberTest(CreateCastMemberTestFixture fixture)
            => _fixture = fixture;
        [Fact(DisplayName = nameof(Create))]
        [Trait("Integration/Application", "CreateCastMember - Use Cases")]
        public async void Create()
        {
            var dbContext = _fixture.CreateDbContext();
            var repository = new CastMemberRepository(dbContext);
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var eventPublisher = new DomainEventPublisher(serviceProvider);
            var unitOfWork = new UnitOfWork(
                dbContext, 
                eventPublisher, 
                serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());
            var useCase = new UseCase.CreateCastMember(repository, unitOfWork);
            var input = _fixture.GetExampleInput();

            var output = await useCase.Handle(input, CancellationToken.None);

            var castMember = await (_fixture.CreateDbContext(true).CastMembers.FindAsync(output.Id));
            castMember.Should().NotBeNull();
            castMember!.Id.Should().NotBeEmpty();
            castMember!.Name.Should().Be(input.Name);
            castMember.Type.Should().Be(input.Type);
            castMember.CreatedAt.Should().Be(output.CreatedAt);
        }

        [Theory(DisplayName = (nameof(ThrowWhenInvalidName)))]
        [Trait("Application", "CreateCastMember - Use Cases")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task ThrowWhenInvalidName(string? name)
        {
            var dbContext = _fixture.CreateDbContext();
            var repository = new CastMemberRepository(dbContext);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var eventPublisher = new DomainEventPublisher(serviceProvider);
            var unitOfWork = new UnitOfWork(
                dbContext,
                eventPublisher,
                serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());

            var useCase = new UseCase.CreateCastMember(repository, unitOfWork);
            var input = new UseCase.CreateCastMemberInput(
                  name!, _fixture.GetRandomCastMemberType());

            var action = async () => await useCase.Handle(input, CancellationToken.None);
            await action.Should().ThrowAsync<EntityValidationException>()
                    .WithMessage("Name should not be null or empty");

        }
    }
}
