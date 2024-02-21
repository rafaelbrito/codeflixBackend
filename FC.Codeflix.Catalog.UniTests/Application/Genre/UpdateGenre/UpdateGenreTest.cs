using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FluentAssertions;
using Moq;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.UniTests.Application.Genre.UpdateGenre
{
    [Collection(nameof(UpdateGenreTestFixture))]
    public class UpdateGenreTest
    {
        private readonly UpdateGenreTestFixture _fixture;

        public UpdateGenreTest(UpdateGenreTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(UpdateGenre))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        public async Task UpdateGenre()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            var exampleGenre = _fixture.GetExampleGenre();
            var newNameExemple = _fixture.GetValidGenreName();
            var newIsActive = !exampleGenre.IsActive;

            genreRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == exampleGenre.Id),
                It.IsAny<CancellationToken>()
               )
            ).ReturnsAsync(exampleGenre);

            var useCase = new UseCase.UpdateGenre(
                genreRepositoryMock.Object,
                unitOfWorkMock.Object,
                _fixture.GetCategoryRepositoryMock().Object
                );

            var input = new UseCase.UpdateGenreInput(
                exampleGenre.Id,
                newNameExemple,
                newIsActive
                );

            GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(exampleGenre.Id);
            output.Name.Should().Be(newNameExemple);
            output.IsActive.Should().Be(newIsActive);
            output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
            output.Categories.Should().HaveCount(0);

            genreRepositoryMock.Verify(
                x => x.Update(
                    It.Is<DomainEntity.Genre>(x => x.Id == exampleGenre.Id),
                    It.IsAny<CancellationToken>()
                    ), Times.Once()
                );

            unitOfWorkMock.Verify(
                x => x.Commit(
                    It.IsAny<CancellationToken>()
                    ), Times.Once()
                );
        }

        [Fact(DisplayName = nameof(ThrowWhenNotFound))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        public async Task ThrowWhenNotFound()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var exampleId = Guid.NewGuid();
            genreRepositoryMock.Setup(x => x.Get(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
               )
            ).ThrowsAsync(new NotFoundException(
                $"Genre '{exampleId}' not found."));

            var useCase = new UseCase.UpdateGenre(
                genreRepositoryMock.Object,
                _fixture.GetUnitOfWorkMock().Object,
                _fixture.GetCategoryRepositoryMock().Object
                );

            var input = new UseCase.UpdateGenreInput(
                exampleId,
                _fixture.GetValidGenreName(),
                true
                );

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>()
               .WithMessage($"Genre '{exampleId}' not found.");
        }

        [Theory(DisplayName = nameof(ThrowWhenNameIsInvalid))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task ThrowWhenNameIsInvalid(string name)
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            var exampleGenre = _fixture.GetExampleGenre();
            var newIsActive = !exampleGenre.IsActive;

            genreRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == exampleGenre.Id),
                It.IsAny<CancellationToken>()
               )
            ).ReturnsAsync(exampleGenre);

            var useCase = new UseCase.UpdateGenre(
                genreRepositoryMock.Object,
                unitOfWorkMock.Object,
                _fixture.GetCategoryRepositoryMock().Object
                );

            var input = new UseCase.UpdateGenreInput(
                exampleGenre.Id,
                name,
                newIsActive
                );

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<EntityValidationException>()
               .WithMessage($"Name should not be null or empty");
        }

        [Theory(DisplayName = nameof(UpdateGenreOnlyName))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task UpdateGenreOnlyName(bool isActive)
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            var exampleGenre = _fixture.GetExampleGenre(isActive);
            var newNameExemple = _fixture.GetValidGenreName();
            var newIsActive = !exampleGenre.IsActive;

            genreRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == exampleGenre.Id),
                It.IsAny<CancellationToken>()
               )
            ).ReturnsAsync(exampleGenre);

            var useCase = new UseCase.UpdateGenre(
                genreRepositoryMock.Object,
                unitOfWorkMock.Object,
                _fixture.GetCategoryRepositoryMock().Object
                );

            var input = new UseCase.UpdateGenreInput(
                exampleGenre.Id,
                newNameExemple
                );

            GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(exampleGenre.Id);
            output.Name.Should().Be(newNameExemple);
            output.IsActive.Should().Be(isActive);
            output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
            output.Categories.Should().HaveCount(0);

            genreRepositoryMock.Verify(
                x => x.Update(
                    It.Is<DomainEntity.Genre>(x => x.Id == exampleGenre.Id),
                    It.IsAny<CancellationToken>()
                    ), Times.Once()
                );

            unitOfWorkMock.Verify(
                x => x.Commit(
                    It.IsAny<CancellationToken>()
                    ), Times.Once()
                );
        }

        [Fact(DisplayName = nameof(UpdateGenreAddingCategoriesId))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        public async Task UpdateGenreAddingCategoriesId()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
            var exampleGenre = _fixture.GetExampleGenre();
            var exampleCategoriesList = _fixture.GetRandomIdsList();
            var newNameExemple = _fixture.GetValidGenreName();
            var newIsActive = !exampleGenre.IsActive;

            genreRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == exampleGenre.Id),
                It.IsAny<CancellationToken>()
               )
            ).ReturnsAsync(exampleGenre);

            categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
               It.IsAny<List<Guid>>(),
               It.IsAny<CancellationToken>()
              )
           ).ReturnsAsync(exampleCategoriesList);

            var useCase = new UseCase.UpdateGenre(
                genreRepositoryMock.Object,
                unitOfWorkMock.Object,
                categoryRepositoryMock.Object
                );

            var input = new UseCase.UpdateGenreInput(
                exampleGenre.Id,
                newNameExemple,
                newIsActive,
                exampleCategoriesList
                );

            GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(exampleGenre.Id);
            output.Name.Should().Be(newNameExemple);
            output.IsActive.Should().Be(newIsActive);
            output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
            output.Categories.Should().HaveCount(exampleCategoriesList.Count);
            exampleCategoriesList.ForEach(
                expectedId => output.Categories.Should().Contain(expectedId)
                );

            genreRepositoryMock.Verify(
                x => x.Update(
                    It.Is<DomainEntity.Genre>(x => x.Id == exampleGenre.Id),
                    It.IsAny<CancellationToken>()
                    ), Times.Once()
                );

            unitOfWorkMock.Verify(
                x => x.Commit(
                    It.IsAny<CancellationToken>()
                    ), Times.Once()
                );
        }

        [Fact(DisplayName = nameof(UpdateGenreReplacingCategoriesId))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        public async Task UpdateGenreReplacingCategoriesId()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
            var exampleGenre = _fixture.GetExampleGenre(categoriesIds: _fixture.GetRandomIdsList());
            var exampleCategoriesList = _fixture.GetRandomIdsList();
            var newNameExemple = _fixture.GetValidGenreName();
            var newIsActive = !exampleGenre.IsActive;

            genreRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == exampleGenre.Id),
                It.IsAny<CancellationToken>()
               )
            ).ReturnsAsync(exampleGenre);

            categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
              It.IsAny<List<Guid>>(),
              It.IsAny<CancellationToken>()
             )
          ).ReturnsAsync(exampleCategoriesList);

            var useCase = new UseCase.UpdateGenre(
                genreRepositoryMock.Object,
                unitOfWorkMock.Object,
                categoryRepositoryMock.Object
                );

            var input = new UseCase.UpdateGenreInput(
                exampleGenre.Id,
                newNameExemple,
                newIsActive,
                exampleCategoriesList
                );

            GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(exampleGenre.Id);
            output.Name.Should().Be(newNameExemple);
            output.IsActive.Should().Be(newIsActive);
            output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
            output.Categories.Should().HaveCount(exampleCategoriesList.Count);
            exampleCategoriesList.ForEach(
                expectedId => output.Categories.Should().Contain(expectedId)
                );

            genreRepositoryMock.Verify(
                x => x.Update(
                    It.Is<DomainEntity.Genre>(x => x.Id == exampleGenre.Id),
                    It.IsAny<CancellationToken>()
                    ), Times.Once()
                );

            unitOfWorkMock.Verify(
                x => x.Commit(
                    It.IsAny<CancellationToken>()
                    ), Times.Once()
                );
        }

        [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        public async Task ThrowWhenCategoryNotFound()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();

            var exampleGenre = _fixture.GetExampleGenre(
                categoriesIds: _fixture.GetRandomIdsList()
                );

            var exampleNewCategoriesList = _fixture.GetRandomIdsList(10);

            var listReturnedCategoryRepository =
                exampleNewCategoriesList.GetRange(0, exampleNewCategoriesList.Count - 2);

            var idsNotReturnedByCategoryRepository =
                exampleNewCategoriesList.GetRange(exampleNewCategoriesList.Count - 2, 2);

            var newIsActive = !exampleGenre.IsActive;
            var newNameExample = _fixture.GetValidGenreName();

            genreRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == exampleGenre.Id),
                It.IsAny<CancellationToken>()
               )
            ).ReturnsAsync(exampleGenre);

            categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>())
                ).ReturnsAsync(listReturnedCategoryRepository);

            var useCase = new UseCase.UpdateGenre(
                genreRepositoryMock.Object,
                unitOfWorkMock.Object,
                categoryRepositoryMock.Object
                );

            var input = new UseCase.UpdateGenreInput(
                exampleGenre.Id,
                newNameExample,
                newIsActive,
                exampleNewCategoriesList
                );

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            var notFoudIdsAsString = String.Join(", ", idsNotReturnedByCategoryRepository);
            await action.Should().ThrowAsync<RelatedAggregateException>()
                .WithMessage($"Related category id (or ids) not found: '{notFoudIdsAsString}'");
        }

        [Fact(DisplayName = nameof(UpdateGenreWithoutCategoriesId))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        public async Task UpdateGenreWithoutCategoriesId()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
            var exampleCategoriesList = _fixture.GetRandomIdsList();
            var exampleGenre = _fixture.GetExampleGenre(categoriesIds: exampleCategoriesList);
            var newNameExemple = _fixture.GetValidGenreName();
            var newIsActive = !exampleGenre.IsActive;

            genreRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == exampleGenre.Id),
                It.IsAny<CancellationToken>()
               )
            ).ReturnsAsync(exampleGenre);

            var useCase = new UseCase.UpdateGenre(
                genreRepositoryMock.Object,
                unitOfWorkMock.Object,
                categoryRepositoryMock.Object
                );

            var input = new UseCase.UpdateGenreInput(
                exampleGenre.Id,
                newNameExemple,
                newIsActive
                );

            GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(exampleGenre.Id);
            output.Name.Should().Be(newNameExemple);
            output.IsActive.Should().Be(newIsActive);
            output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
            output.Categories.Should().HaveCount(exampleCategoriesList.Count);
            exampleCategoriesList.ForEach(
                expectedId => output.Categories.Should().Contain(expectedId)
                );

            genreRepositoryMock.Verify(
                x => x.Update(
                    It.Is<DomainEntity.Genre>(x => x.Id == exampleGenre.Id),
                    It.IsAny<CancellationToken>()
                    ), Times.Once()
                );

            unitOfWorkMock.Verify(
                x => x.Commit(
                    It.IsAny<CancellationToken>()
                    ), Times.Once()
                );
        }

        [Fact(DisplayName = nameof(UpdateGenreWithEmptyCategoriesIdList))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        public async Task UpdateGenreWithEmptyCategoriesIdList()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
            var exampleCategoriesList = _fixture.GetRandomIdsList();
            var exampleGenre = _fixture.GetExampleGenre(categoriesIds: exampleCategoriesList);
            var newNameExemple = _fixture.GetValidGenreName();
            var newIsActive = !exampleGenre.IsActive;

            genreRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == exampleGenre.Id),
                It.IsAny<CancellationToken>()
               )
            ).ReturnsAsync(exampleGenre);

            var useCase = new UseCase.UpdateGenre(
                genreRepositoryMock.Object,
                unitOfWorkMock.Object,
                categoryRepositoryMock.Object
                );

            var input = new UseCase.UpdateGenreInput(
                exampleGenre.Id,
                newNameExemple,
                newIsActive,
                new List<Guid>()
                );

            GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(exampleGenre.Id);
            output.Name.Should().Be(newNameExemple);
            output.IsActive.Should().Be(newIsActive);
            output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
            output.Categories.Should().HaveCount(0);

            genreRepositoryMock.Verify(
                x => x.Update(
                    It.Is<DomainEntity.Genre>(x => x.Id == exampleGenre.Id),
                    It.IsAny<CancellationToken>()
                    ), Times.Once()
                );

            unitOfWorkMock.Verify(
                x => x.Commit(
                    It.IsAny<CancellationToken>()
                    ), Times.Once()
                );
        }
    }
}
