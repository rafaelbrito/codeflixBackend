using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using Xunit;
using FluentAssertions;
using Bogus.Extensions.UnitedKingdom;
using FC.Codeflix.Catalog.Infra.Data.EF.Model;
using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.CreateGenre
{
    [Collection(nameof(CreateGenreTestFixture))]
    public class CreateGenreTest
    {
        private readonly CreateGenreTestFixture _fixture;
        public CreateGenreTest(CreateGenreTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(CreateGenre))]
        [Trait("Integration/Application", "CreateGenre - Use Cases")]
        public async Task CreateGenre()
        {
            var dbContext = _fixture.CreateDbContext();
            var genreRepository = new GenreRepository(dbContext);
            var categoryRepository = new CategoryRespository(dbContext);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var eventPublisher = new DomainEventPublisher(serviceProvider);
            var unitOfWork = new UnitOfWork(
                dbContext,
                eventPublisher,
                serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());

            var useCase = new UseCase.CreateGenre(genreRepository, unitOfWork, categoryRepository);
            var input = _fixture.GetExampleInput();
            var output = await useCase.Handle(input, CancellationToken.None);

            output.Id.Should().NotBeEmpty();
            output!.Name.Should().Be(input.Name);
            output.IsActive.Should().Be(input.IsActive);
            output.CreatedAt.Should().NotBe(default);
            output.Categories.Should().HaveCount(0);

            var assertDbContext = _fixture.CreateDbContext(true);
            var genre = await assertDbContext.Genres.FindAsync(output.Id);
            genre.Should().NotBeNull();
            genre!.Id.Should().NotBeEmpty();
            genre.Name.Should().Be(input.Name);
            genre.IsActive.Should().Be(input.IsActive);
            genre.CreatedAt.Should().NotBe(default);
            genre.Categories.Should().HaveCount(0);
        }

        [Fact(DisplayName = nameof(CreateGenreWithCategoriesRelations))]
        [Trait("Integration/Application", "CreateGenre - Use Cases")]
        public async Task CreateGenreWithCategoriesRelations()
        {
            var exampleCategories = _fixture.GetExampleCategoryList(5);
            var dbContext = _fixture.CreateDbContext();
            await dbContext.Categories.AddRangeAsync(exampleCategories);
            await dbContext.SaveChangesAsync();
            var input = _fixture.GetExampleInput();
            input.CategoriesIds = exampleCategories
                .Select(category => category.Id).ToList();
            var genreRepository = new GenreRepository(dbContext);
            var categoryRepository = new CategoryRespository(dbContext);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var eventPublisher = new DomainEventPublisher(serviceProvider);
            var unitOfWork = new UnitOfWork(
                dbContext,
                eventPublisher,
                serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());

            var useCase = new UseCase.CreateGenre(genreRepository, unitOfWork, categoryRepository);
            var output = await useCase.Handle(input, CancellationToken.None);

            output.Id.Should().NotBeEmpty();
            output!.Name.Should().Be(input.Name);
            output.IsActive.Should().Be(input.IsActive);
            output.CreatedAt.Should().NotBe(default);
            output.Categories.Should().HaveCount(input.CategoriesIds.Count);
            output.Categories.Select(relation => relation.Id)
                .ToList().Should().BeEquivalentTo(input.CategoriesIds);
            var assertDbContext = _fixture.CreateDbContext(true);
            var genre = await assertDbContext.Genres.FindAsync(output.Id);
            genre.Should().NotBeNull();
            genre!.Name.Should().Be(input.Name);
            genre.IsActive.Should().Be(input.IsActive);
            var relations = await assertDbContext.GenresCategories.AsNoTracking().
                Where(x => x.GenreId == output.Id).ToListAsync();
            relations.Should().HaveCount(input.CategoriesIds.Count);
            relations.Select(relation => relation.CategoryId)
                .ToList().Should().BeEquivalentTo(input.CategoriesIds);
        }

        [Fact(DisplayName = nameof(CreateGenreThrowWhenCategoryDoesntExists))]
        [Trait("Integration/Application", "CreateGenre - Use Cases")]
        public async Task CreateGenreThrowWhenCategoryDoesntExists()
        {
            var exampleCategories = _fixture.GetExampleCategoryList(5);
            var dbContext = _fixture.CreateDbContext();
            await dbContext.Categories.AddRangeAsync(exampleCategories);
            await dbContext.SaveChangesAsync();
            var input = _fixture.GetExampleInput();
            input.CategoriesIds = exampleCategories
                .Select(category => category.Id).ToList();
            var randomGuid = Guid.NewGuid();
            input.CategoriesIds.Add(randomGuid);
            var genreRepository = new GenreRepository(dbContext);
            var categoryRepository = new CategoryRespository(dbContext);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var eventPublisher = new DomainEventPublisher(serviceProvider);
            var unitOfWork = new UnitOfWork(
                dbContext,
                eventPublisher,
                serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());

            var useCase = new UseCase.CreateGenre(genreRepository, unitOfWork, categoryRepository);
            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<RelatedAggregateException>()
                .WithMessage($"Related category id (or ids) not found: '{randomGuid}'");
        }
    }
}
