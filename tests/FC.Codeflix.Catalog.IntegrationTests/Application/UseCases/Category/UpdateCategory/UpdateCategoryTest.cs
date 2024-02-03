using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using Xunit;
using FluentAssertions;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.UpdateCategory
{
    [Collection(nameof(UpdateCategoryTestFixture))]
    public class UpdateCategoryTest
    {
        private readonly UpdateCategoryTestFixture _fixture;
        public UpdateCategoryTest(UpdateCategoryTestFixture fixture)
                => _fixture = fixture;

        [Theory(DisplayName = (nameof(UpdateCategory)))]
        [Trait("Integration/Application", "UpdateCategory - Use Cases")]
        [MemberData(nameof(UpdateCategoryDataGenerator.GetCategotyToUpdate),
            parameters: 5, MemberType = typeof(UpdateCategoryDataGenerator))]
        public async Task UpdateCategory(
            DomainEntity.Category exampleCategory, UpdateCategoryInput input)
        {
            var dbContext = _fixture.CreateDbContext();
            await dbContext.AddRangeAsync(_fixture.GetExampleCategoryList());
            var trackingInfo = await dbContext.AddAsync(exampleCategory);
            dbContext.SaveChanges();
            trackingInfo.State = EntityState.Detached;
            var repository = new CategoryRespository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var useCase = new UseCase.UpdateCategory(repository, unitOfWork);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be((bool)input.IsActive!);

            var dbCategory = await (_fixture.CreateDbContext(true))
                .Categories.FindAsync(output.Id);
            dbCategory!.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(input.Description);
            dbCategory.IsActive.Should().Be((bool)input.IsActive!);
            dbCategory.CreatedAt.Should().Be(output.CreatedAt);
        }

        [Theory(DisplayName = (nameof(UpdateCategoryWithoutIsActive)))]
        [Trait("Integration/Application", "UpdateCategory - Use Cases")]
        [MemberData(nameof(UpdateCategoryDataGenerator.GetCategotyToUpdate),
            parameters: 5, MemberType = typeof(UpdateCategoryDataGenerator))]
        public async Task UpdateCategoryWithoutIsActive(
            DomainEntity.Category exampleCategory, UpdateCategoryInput exampleInput)
        {
            var input = new UpdateCategoryInput(
                exampleInput.Id,
                exampleInput.Name,
                exampleInput.Description
                );
            var dbContext = _fixture.CreateDbContext();
            await dbContext.AddRangeAsync(_fixture.GetExampleCategoryList());
            var trackingInfo = await dbContext.AddAsync(exampleCategory);
            dbContext.SaveChanges();
            trackingInfo.State = EntityState.Detached;
            var repository = new CategoryRespository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var useCase = new UseCase.UpdateCategory(repository, unitOfWork);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be(exampleCategory.IsActive);

            var dbCategory = await (_fixture.CreateDbContext(true))
                .Categories.FindAsync(output.Id);
            dbCategory!.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(input.Description);
            dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
            dbCategory.CreatedAt.Should().Be(output.CreatedAt);
        }

        [Theory(DisplayName = (nameof(UpdateCategoryOnlyName)))]
        [Trait("Integration/Application", "UpdateCategory - Use Cases")]
        [MemberData(nameof(UpdateCategoryDataGenerator.GetCategotyToUpdate),
           parameters: 5, MemberType = typeof(UpdateCategoryDataGenerator))]
        public async Task UpdateCategoryOnlyName(
            DomainEntity.Category exampleCategory, UpdateCategoryInput exampleInput)
        {
            var input = new UpdateCategoryInput(
                exampleInput.Id,
                exampleInput.Name
                );
            var dbContext = _fixture.CreateDbContext();
            await dbContext.AddRangeAsync(_fixture.GetExampleCategoryList());
            var trackingInfo = await dbContext.AddAsync(exampleCategory);
            dbContext.SaveChanges();
            trackingInfo.State = EntityState.Detached;
            var repository = new CategoryRespository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var useCase = new UseCase.UpdateCategory(repository, unitOfWork);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(exampleCategory.Description);
            output.IsActive.Should().Be(exampleCategory.IsActive);

            var dbCategory = await (_fixture.CreateDbContext(true))
                .Categories.FindAsync(output.Id);
            dbCategory!.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(exampleCategory.Description);
            dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
            dbCategory.CreatedAt.Should().Be(output.CreatedAt);
        }

        [Fact(DisplayName = (nameof(UpdateThrowsWhenNotFoundCategory)))]
        [Trait("Integration/Application", "UpdateCategory - Use Cases")]
        public async Task UpdateThrowsWhenNotFoundCategory()
        {
            var input = _fixture.GetValidInput();
            var dbContext = _fixture.CreateDbContext();
            await dbContext.AddRangeAsync(_fixture.GetExampleCategoryList());
            dbContext.SaveChanges();
            var repository = new CategoryRespository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var useCase = new UseCase.UpdateCategory(repository, unitOfWork);

            var task = async ()
                => await useCase.Handle(input, CancellationToken.None);

            await task.Should().ThrowAsync<NotFoundException>()
                 .WithMessage($"Category '{input.Id}' not found.");
        }

        [Theory(DisplayName = (nameof(UpdateThrowsWhenCantInstantiateCategory)))]
        [Trait("Integration/Application", "UpdateCategory - Use Cases")]
        [MemberData(nameof(UpdateCategoryDataGenerator.GetInvalidInputs),
          parameters: 6, MemberType = typeof(UpdateCategoryDataGenerator))]
        public async Task UpdateThrowsWhenCantInstantiateCategory(
            UpdateCategoryInput input, string exceptionMessage)
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleCategory = _fixture.GetExampleCategoryList();
            await dbContext.AddRangeAsync(exampleCategory);
            dbContext.SaveChanges();
            var repository = new CategoryRespository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var useCase = new UseCase.UpdateCategory(repository, unitOfWork);
            input.Id = exampleCategory[0].Id;

            var task = async () 
                => await useCase.Handle(input, CancellationToken.None);

            await task.Should().ThrowAsync<EntityValidationException>()
                .WithMessage(exceptionMessage);
        }
    }
}
