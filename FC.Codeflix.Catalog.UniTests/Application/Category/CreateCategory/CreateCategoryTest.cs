using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;

namespace FC.Codeflix.Catalog.UniTests.Application.Category.CreateCategory
{
    [Collection(nameof(CreateCategoryTestFixture))]
    public class CreateCategoryTest
    {
        private readonly CreateCategoryTestFixture _fixture;
        public CreateCategoryTest(CreateCategoryTestFixture fixture)
        {
            _fixture = fixture;
        }


        [Fact(DisplayName = nameof(CreateCategory))]
        [Trait("Application", "CreateCategory - Use Cases")]
        public async void CreateCategory()
        {

            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

            var useCase = new UseCase.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object);

            var input = _fixture.GetInput();
            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(repository =>
            repository.Insert(It.IsAny<DomainEntity.Category>(),
            It.IsAny<CancellationToken>()), Times.Once);

            unitOfWorkMock.Verify(uow =>
            uow.Commit(It.IsAny<CancellationToken>()), Times.Once);

            output.Should().NotBeNull();
            output.Id.Should().NotBeEmpty();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be(input.IsActive);
            output.CreatedAt.Should().NotBeSameDateAs(default);
        }

        [Fact(DisplayName = nameof(CreateCategoryWithOnlyName))]
        [Trait("Application", "CreateCategory - Use Cases")]
        public async void CreateCategoryWithOnlyName()
        {
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

            var useCase = new UseCase.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object);

            var input = new CreateCategoryInput(
                _fixture.GetValidCategoryName()
                );
            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(repository =>
            repository.Insert(It.IsAny<DomainEntity.Category>(),
            It.IsAny<CancellationToken>()), Times.Once);

            unitOfWorkMock.Verify(uow =>
            uow.Commit(It.IsAny<CancellationToken>()), Times.Once);

            output.Should().NotBeNull();
            output.Id.Should().NotBeEmpty();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be("");
            output.IsActive.Should().Be(input.IsActive);
            output.CreatedAt.Should().NotBeSameDateAs(default);
        }

        [Fact(DisplayName = nameof(CreateCategoryWithOnlyNameAndDescription))]
        [Trait("Application", "CreateCategory - Use Cases")]
        public async void CreateCategoryWithOnlyNameAndDescription()
        {
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

            var useCase = new UseCase.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object);

            var input = new CreateCategoryInput(
                _fixture.GetValidCategoryName(),
                _fixture.GetValidCategoryDescription()
                );
            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(repository =>
            repository.Insert(It.IsAny<DomainEntity.Category>(),
            It.IsAny<CancellationToken>()), Times.Once);

            unitOfWorkMock.Verify(uow =>
            uow.Commit(It.IsAny<CancellationToken>()), Times.Once);

            output.Should().NotBeNull();
            output.Id.Should().NotBeEmpty();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be(input.IsActive);
            output.CreatedAt.Should().NotBeSameDateAs(default);
        }

        [Theory(DisplayName = nameof(ThrowWhenCanInstatiateCategory))]
        [Trait("Application", "CreateCategory - Use Cases")]
        [MemberData(
            nameof(CreateCategoryTestDataGenerator.GetInvalidInputs),
            parameters: 12,
            MemberType = typeof(CreateCategoryTestDataGenerator))]
        public async void ThrowWhenCanInstatiateCategory(CreateCategoryInput input, string exceptionMessage)
        {
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

            var useCase = new UseCase.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object);
            Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);
            await task.Should()
                .ThrowAsync<EntityValidationException>()
                .WithMessage(exceptionMessage);
        }
    }
}
