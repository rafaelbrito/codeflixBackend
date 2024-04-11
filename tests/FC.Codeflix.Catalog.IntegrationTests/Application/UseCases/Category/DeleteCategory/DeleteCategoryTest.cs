using FC.Codeflix.Catalog.Application.Exceptions;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;
using FluentAssertions;
using Xunit;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.DeleteCategory
{
    [Collection(nameof(DeleteCategoryTestFixture))]
    public class DeleteCategoryTest
    {
        private readonly DeleteCategoryTestFixture _fixture;
        public DeleteCategoryTest(DeleteCategoryTestFixture fixture)
                => _fixture = fixture;

        [Fact(DisplayName = nameof(DeleteCategory))]
        [Trait("Integration/Application", "DeleteCategory - Use Cases")]
        public async Task DeleteCategory()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleCategory = _fixture.GetExampleCategory();
            await dbContext.AddRangeAsync(_fixture.GetExampleCategoryList(10));
            var tracking = await dbContext.AddAsync(exampleCategory);
            dbContext.SaveChanges();
            tracking.State = EntityState.Detached;
            var repository = new CategoryRespository(dbContext);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var eventPublisher = new DomainEventPublisher(serviceProvider);
            var unitOfWork = new UnitOfWork(
                dbContext,
                eventPublisher,
                serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());

            var input = new UseCase.DeleteCategoryInput(exampleCategory.Id);

            var useCase = new UseCase.DeleteCategory(repository, unitOfWork);
            await useCase.Handle(input, CancellationToken.None);

            var assertDbContext = _fixture.CreateDbContext(true);
            var dbCategoryDeleted = await assertDbContext.Categories.FindAsync(exampleCategory.Id);
            dbCategoryDeleted.Should().BeNull();
            var dbCategories = await assertDbContext.Categories.ToListAsync();
            dbCategories.Should().HaveCount(10);
        }

        [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
        [Trait("Integration/Application", "DeleteCategory - Use Cases")]
        public async Task ThrowWhenCategoryNotFound()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleCategory = _fixture.GetExampleCategory();
            await dbContext.AddRangeAsync(_fixture.GetExampleCategoryList(10));
            var tracking = await dbContext.AddAsync(exampleCategory);
            dbContext.SaveChanges();
            tracking.State = EntityState.Detached;
            var repository = new CategoryRespository(dbContext);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var eventPublisher = new DomainEventPublisher(serviceProvider);
            var unitOfWork = new UnitOfWork(
                dbContext,
                eventPublisher,
                serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());

            var input = new UseCase.DeleteCategoryInput(Guid.NewGuid());

            var useCase = new UseCase.DeleteCategory(repository, unitOfWork);
            var task = async () => await useCase.Handle(input, CancellationToken.None);

            await task.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Category '{input.Id}' not found.");
        }
    }
}
