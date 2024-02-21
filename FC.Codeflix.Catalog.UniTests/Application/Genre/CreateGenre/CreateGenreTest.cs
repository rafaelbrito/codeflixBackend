using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using Moq;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Exceptions;
namespace FC.Codeflix.Catalog.UniTests.Application.Genre.CreateGenre
{
    [Collection(nameof(CreateGenreTestFixture))]
    public class CreateGenreTest
    {
        private readonly CreateGenreTestFixture _fixture;

        public CreateGenreTest(CreateGenreTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(Create))]
        [Trait("Application", "CreateGenre - Use Cases")]
        public async void Create()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
            var useCase = new UseCase.CreateGenre(
                genreRepositoryMock.Object,
                unitOfWorkMock.Object,
                categoryRepositoryMock.Object
                );
            var input = _fixture.GetExampleInput();

            var output = await
                useCase.Handle(input, CancellationToken.None);

            genreRepositoryMock.Verify(x => x.Insert(
                It.IsAny<DomainEntity.Genre>(),
                It.IsAny<CancellationToken>()
                ), Times.Once
                    );

            unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()
                ), Times.Once
                    );

            output.Id.Should().NotBeEmpty();
            output.Name.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.IsActive.Should().Be(input.IsActive);
            output.Categories.Should().HaveCount(0);
            output.CreatedAt.Should().NotBeSameDateAs(default);
        }

        [Fact(DisplayName = nameof(CreateWithRelatedCategories))]
        [Trait("Application", "CreateGenre - Use Cases")]
        public async void CreateWithRelatedCategories()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
            var useCase = new UseCase.CreateGenre(
                genreRepositoryMock.Object,
                unitOfWorkMock.Object,
                categoryRepositoryMock.Object
                );
            var input = _fixture.GetExampleInputWithCategories();

            categoryRepositoryMock.Setup(
               x => x.GetIdsListByIds(
                   It.IsAny<List<Guid>>(),
                   It.IsAny<CancellationToken>()
                   )
               ).ReturnsAsync(
                   (IReadOnlyList<Guid>)input.CategoriesIds!);

            var output = await
                useCase.Handle(input, CancellationToken.None);

            genreRepositoryMock.Verify(x => x.Insert(
                It.IsAny<DomainEntity.Genre>(),
                It.IsAny<CancellationToken>()
                ), Times.Once
                    );

            unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()
                ), Times.Once
                    );

            output.Id.Should().NotBeEmpty();
            output.Name.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.IsActive.Should().Be(input.IsActive);
            output.Categories.Should().HaveCount(input.CategoriesIds?.Count ?? 0);
            input.CategoriesIds?.ForEach(
                id => output.Categories.Should().Contain(id)
                );
            output.CreatedAt.Should().NotBeSameDateAs(default);
        }

        [Fact(DisplayName = nameof(CreateThrowWhenRelatedCategoryNotFound))]
        [Trait("Application", "CreateGenre - Use Cases")]
        public async void CreateThrowWhenRelatedCategoryNotFound()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

            var input = _fixture.GetExampleInputWithCategories();
            var exampleGuid = input.CategoriesIds![^1];

            categoryRepositoryMock.Setup(
                x => x.GetIdsListByIds(
                    It.IsAny<List<Guid>>(),
                    It.IsAny<CancellationToken>()
                    )
                ).ReturnsAsync(
                    (IReadOnlyList<Guid>)input.CategoriesIds
                    .FindAll(x => x != exampleGuid)
                );

            var useCase = new UseCase.CreateGenre(
                genreRepositoryMock.Object,
                unitOfWorkMock.Object,
                categoryRepositoryMock.Object
                );

            var action = async () =>
                await useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<RelatedAggregateException>()
                .WithMessage($"Related category id (or ids) not found: '{exampleGuid}'");

            categoryRepositoryMock.Verify(
                x => x.GetIdsListByIds(
                    It.IsAny<List<Guid>>(),
                    It.IsAny<CancellationToken>()
                    ), Times.Once
                );



        }

        [Theory(DisplayName = nameof(ThrowWhenNameInsvalid))]
        [Trait("Application", "CreateGenre - Use Cases")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async void ThrowWhenNameInsvalid(string name)
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

            var input = _fixture.GetExampleInput(name);

            var useCase = new UseCase.CreateGenre(
                genreRepositoryMock.Object,
                unitOfWorkMock.Object,
                categoryRepositoryMock.Object
                );

            var action = async () =>
                await useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<EntityValidationException>()
                .WithMessage($"Name should not be null or empty");
        }
    }
}
