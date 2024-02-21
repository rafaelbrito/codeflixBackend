using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.CreateCategory
{
    [Collection(nameof(CreateCategoryTestFixture))]
    public class CreateCategoryTest
    {
        private readonly CreateCategoryTestFixture _fixture;
        public CreateCategoryTest(CreateCategoryTestFixture fixture)
        => _fixture = fixture;

        [Fact(DisplayName = nameof(CreateCategory))]
        [Trait("Integration/Application", "CreateCategory - Use Cases")]
        public async void CreateCategory()
        {

            var dbContext = _fixture.CreateDbContext();
            var repository = new CategoryRespository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);

            var useCase = new UseCase.CreateCategory(repository, unitOfWork);

            var input = _fixture.GetInput();
            var output = await useCase.Handle(input, CancellationToken.None);


            var dbCategory = await (_fixture.CreateDbContext(true)).Categories.FindAsync(output.Id);
            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(input.Description);
            dbCategory.IsActive.Should().Be(input.IsActive);
            dbCategory.CreatedAt.Should().Be(output.CreatedAt);

            output.Should().NotBeNull();
            output.Id.Should().NotBeEmpty();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be(input.IsActive);
            output.CreatedAt.Should().NotBeSameDateAs(default);


        }

        [Fact(DisplayName = nameof(CreateCategoryOnlyWithName))]
        [Trait("Integration/Application", "CreateCategory - Use Cases")]
        public async void CreateCategoryOnlyWithName()
        {

            var dbContext = _fixture.CreateDbContext();
            var repository = new CategoryRespository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);

            var useCase = new UseCase.CreateCategory(repository, unitOfWork);

            var input = new UseCase.CreateCategoryInput(_fixture.GetValidCategoryName());

            var output = await useCase.Handle(input, CancellationToken.None);


            var dbCategory = await (_fixture.CreateDbContext(true)).Categories.FindAsync(output.Id);
            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be("");
            dbCategory.IsActive.Should().Be(true);
            dbCategory.CreatedAt.Should().Be(output.CreatedAt);

            output.Should().NotBeNull();
            output.Id.Should().NotBeEmpty();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be("");
            output.IsActive.Should().Be(true);
            output.CreatedAt.Should().NotBeSameDateAs(default);


        }

        [Fact(DisplayName = nameof(CreateCategoryOnlyWithNameAndDescription))]
        [Trait("Integration/Application", "CreateCategory - Use Cases")]
        public async void CreateCategoryOnlyWithNameAndDescription()
        {

            var dbContext = _fixture.CreateDbContext();
            var repository = new CategoryRespository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);

            var useCase = new UseCase.CreateCategory(repository, unitOfWork);

            var input = new UseCase.CreateCategoryInput(
                _fixture.GetValidCategoryName(),
                _fixture.GetValidCategoryDescription()
                );

            var output = await useCase.Handle(input, CancellationToken.None);


            var dbCategory = await (_fixture.CreateDbContext(true)).Categories.FindAsync(output.Id);
            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(input.Description);
            dbCategory.IsActive.Should().Be(true);
            dbCategory.CreatedAt.Should().Be(output.CreatedAt);

            output.Should().NotBeNull();
            output.Id.Should().NotBeEmpty();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be(true);
            output.CreatedAt.Should().NotBeSameDateAs(default);
        }


        [Theory(DisplayName = nameof(ThrowWhenCanInstatiateCategory))]
        [Trait("Integration/Application", "CreateCategory - Use Cases")]
        [MemberData(
            nameof(CreateCategoryTestDataGenerator.GetInvalidInputs),
            parameters: 4,
            MemberType = typeof(CreateCategoryTestDataGenerator))]
        public async void ThrowWhenCanInstatiateCategory(UseCase.CreateCategoryInput input, string exceptionMessage)
        {
            var dbContext = _fixture.CreateDbContext();
            var repository = new CategoryRespository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);

            var useCase = new UseCase.CreateCategory(repository, unitOfWork);

            var task = async ()
                => await useCase.Handle(input, CancellationToken.None);
            await task.Should().ThrowAsync<EntityValidationException>()
                .WithMessage(exceptionMessage);
        }
    }
}
