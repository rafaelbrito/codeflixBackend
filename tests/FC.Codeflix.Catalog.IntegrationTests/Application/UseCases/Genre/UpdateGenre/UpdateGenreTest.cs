using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Model;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.UpdateGenre
{
    [Collection(nameof(UpdateGenreTestFixture))]
    public class UpdateGenreTest
    {
        private readonly UpdateGenreTestFixture _fixture;
        public UpdateGenreTest(UpdateGenreTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(UpdateGenre))]
        [Trait("Integration/Application", "UpdateGenre - Use Cases")]
        public async Task UpdateGenre()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleGenre = _fixture.GetExampleListGenres(10);
            var targetGenre = exampleGenre[5];
            await dbContext.AddRangeAsync(exampleGenre);
            await dbContext.SaveChangesAsync();
            var arrangeDbContext = _fixture.CreateDbContext(true);
            UseCase.UpdateGenre updateGenre = new UseCase.UpdateGenre(
                new GenreRepository(arrangeDbContext),
                new UnitOfWork(arrangeDbContext),
                new CategoryRespository(arrangeDbContext)
                );

            var input = new UpdateGenreInput(
                targetGenre.Id,
                _fixture.GetValidGenreName(),
                !targetGenre.IsActive);

            var output = await updateGenre.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(targetGenre.Id);
            output.Name.Should().Be(input.Name);
            output.IsActive.Should().Be((bool)input.IsActive!);
            var assertDbContext = _fixture.CreateDbContext(true);
            var genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
            genreFromDb.Should().NotBeNull();
            genreFromDb!.Id.Should().Be(targetGenre.Id);
            genreFromDb!.Name.Should().Be(output.Name);
            genreFromDb.IsActive.Should().Be(output.IsActive);
        }

        [Fact(DisplayName = nameof(UpdateGenreWithCategoriesRelations))]
        [Trait("Integration/Application", "UpdateGenre - Use Cases")]
        public async Task UpdateGenreWithCategoriesRelations()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleCategories = _fixture.GetExampleCategoryList(10);
            var exampleGenre = _fixture.GetExampleListGenres(10);
            var targetGenre = exampleGenre[5];
            var relatedCategories = exampleCategories.GetRange(0, 5);
            var newRelatedCategories = exampleCategories.GetRange(5, 3);
            relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
            var relations = targetGenre.Categories
                .Select(categoryId => new GenresCategories(categoryId, targetGenre.Id)).ToList();
            await dbContext.AddRangeAsync(exampleGenre);
            await dbContext.AddRangeAsync(exampleCategories);
            await dbContext.AddRangeAsync(relations);
            await dbContext.SaveChangesAsync();
            var arrangeDbContext = _fixture.CreateDbContext(true);
            UseCase.UpdateGenre updateGenre = new UseCase.UpdateGenre(
                new GenreRepository(arrangeDbContext),
                new UnitOfWork(arrangeDbContext),
                new CategoryRespository(arrangeDbContext)
                );

            var input = new UpdateGenreInput(
                targetGenre.Id,
                _fixture.GetValidGenreName(),
                !targetGenre.IsActive,
                newRelatedCategories.Select(category => category.Id).ToList()
                );

            var output = await updateGenre.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(targetGenre.Id);
            output.Name.Should().Be(input.Name);
            output.IsActive.Should().Be((bool)input.IsActive!);
            output.Categories.Should().HaveCount(newRelatedCategories.Count);
            var relatedCategoryIdsFromOutput = output.Categories.Select(relationCategory => relationCategory.Id).ToList();
            relatedCategoryIdsFromOutput.Should().BeEquivalentTo(input.CategoriesIds);
            var assertDbContext = _fixture.CreateDbContext(true);
            var genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
            genreFromDb.Should().NotBeNull();
            genreFromDb!.Id.Should().Be(targetGenre.Id);
            genreFromDb!.Name.Should().Be(output.Name);
            genreFromDb.IsActive.Should().Be(output.IsActive);
            var relatedCategoryIdsFromDb = await assertDbContext.GenresCategories.AsNoTracking()
                .Where(relation => relation.GenreId == input.Id)
                .Select(relation => relation.CategoryId).ToListAsync();
            relatedCategoryIdsFromDb.Should().BeEquivalentTo(input.CategoriesIds);
        }

        [Fact(DisplayName = nameof(UpdateGenreThrowsWhenCategoryDoesntExists))]
        [Trait("Integration/Application", "UpdateGenre - Use Cases")]
        public async Task UpdateGenreThrowsWhenCategoryDoesntExists()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleCategories = _fixture.GetExampleCategoryList(10);
            var exampleGenre = _fixture.GetExampleListGenres(10);
            var targetGenre = exampleGenre[5];
            var relatedCategories = exampleCategories.GetRange(0, 5);
            var newRelatedCategories = exampleCategories.GetRange(5, 3);
            relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
            var relations = targetGenre.Categories
                .Select(categoryId => new GenresCategories(categoryId, targetGenre.Id)).ToList();
            await dbContext.AddRangeAsync(exampleGenre);
            await dbContext.AddRangeAsync(exampleCategories);
            await dbContext.AddRangeAsync(relations);
            await dbContext.SaveChangesAsync();
            var arrangeDbContext = _fixture.CreateDbContext(true);
            UseCase.UpdateGenre updateGenre = new UseCase.UpdateGenre(
                new GenreRepository(arrangeDbContext),
                new UnitOfWork(arrangeDbContext),
                new CategoryRespository(arrangeDbContext)
                );

            var categoryIdsToRelate = newRelatedCategories.Select(category => category.Id).ToList();
            var invalidCategoryId = Guid.NewGuid();
            categoryIdsToRelate.Add(invalidCategoryId);
            var input = new UpdateGenreInput(
                targetGenre.Id,
                _fixture.GetValidGenreName(),
                !targetGenre.IsActive,
                categoryIdsToRelate
                );

            var action = async () => await updateGenre.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<RelatedAggregateException>()
                .WithMessage($"Related category id (or ids) not found: '{invalidCategoryId}'");
        }

        [Fact(DisplayName = nameof(UpdateGenreThrowsWhenNotFound))]
        [Trait("Integration/Application", "UpdateGenre - Use Cases")]
        public async Task UpdateGenreThrowsWhenNotFound()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleGenre = _fixture.GetExampleListGenres(10);
            var exampleGuid = Guid.NewGuid();
            await dbContext.AddRangeAsync(exampleGenre);
            await dbContext.SaveChangesAsync();
            var arrangeDbContext = _fixture.CreateDbContext(true);
            UseCase.UpdateGenre updateGenre = new UseCase.UpdateGenre(
                new GenreRepository(arrangeDbContext),
                new UnitOfWork(arrangeDbContext),
                new CategoryRespository(arrangeDbContext)
                );

            var input = new UpdateGenreInput(
                exampleGuid,
                _fixture.GetValidGenreName(),
                true
                );

            var action = async () => await updateGenre.Handle(input, CancellationToken.None);
            await action.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Genre '{exampleGuid}' not found.");
        }

        [Fact(DisplayName = nameof(UpdateGenreWithoutNewCategoriesRelations))]
        [Trait("Integration/Application", "UpdateGenre - Use Cases")]
        public async Task UpdateGenreWithoutNewCategoriesRelations()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleCategories = _fixture.GetExampleCategoryList(10);
            var exampleGenre = _fixture.GetExampleListGenres(10);
            var targetGenre = exampleGenre[5];
            var relatedCategories = exampleCategories.GetRange(0, 5);
            relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
            var relations = targetGenre.Categories
                .Select(categoryId => new GenresCategories(categoryId, targetGenre.Id)).ToList();
            await dbContext.AddRangeAsync(exampleGenre);
            await dbContext.AddRangeAsync(exampleCategories);
            await dbContext.AddRangeAsync(relations);
            await dbContext.SaveChangesAsync();
            var arrangeDbContext = _fixture.CreateDbContext(true);
            UseCase.UpdateGenre updateGenre = new UseCase.UpdateGenre(
                new GenreRepository(arrangeDbContext),
                new UnitOfWork(arrangeDbContext),
                new CategoryRespository(arrangeDbContext)
                );

            var input = new UpdateGenreInput(
                targetGenre.Id,
                _fixture.GetValidGenreName(),
                !targetGenre.IsActive
                );

            var output = await updateGenre.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(targetGenre.Id);
            output.Name.Should().Be(input.Name);
            output.IsActive.Should().Be((bool)input.IsActive!);
            output.Categories.Should().HaveCount(relatedCategories.Count);
            var expectedRelatedCategoryIds = output.Categories
                .Select(relatedCategory => relatedCategory.Id).ToList();  
            var relatedCategoryIdsFromOutput = output.Categories.Select(relationCategory => relationCategory.Id).ToList();
            relatedCategoryIdsFromOutput.Should().BeEquivalentTo(expectedRelatedCategoryIds);
            var assertDbContext = _fixture.CreateDbContext(true);
            var genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
            genreFromDb.Should().NotBeNull();
            genreFromDb!.Id.Should().Be(targetGenre.Id);
            genreFromDb!.Name.Should().Be(output.Name);
            genreFromDb.IsActive.Should().Be(output.IsActive);
            var relatedCategoryIdsFromDb = await assertDbContext.GenresCategories.AsNoTracking()
                .Where(relation => relation.GenreId == input.Id)
                .Select(relation => relation.CategoryId).ToListAsync();
            relatedCategoryIdsFromDb.Should().BeEquivalentTo(expectedRelatedCategoryIds);
        }

        [Fact(DisplayName = nameof(UpdateGenreWithEmptyCategoriesClearRelations))]
        [Trait("Integration/Application", "UpdateGenre - Use Cases")]
        public async Task UpdateGenreWithEmptyCategoriesClearRelations()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleCategories = _fixture.GetExampleCategoryList(10);
            var exampleGenre = _fixture.GetExampleListGenres(10);
            var targetGenre = exampleGenre[5];
            var relatedCategories = exampleCategories.GetRange(0, 5);
            relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
            var relations = targetGenre.Categories
                .Select(categoryId => new GenresCategories(categoryId, targetGenre.Id)).ToList();
            await dbContext.AddRangeAsync(exampleGenre);
            await dbContext.AddRangeAsync(exampleCategories);
            await dbContext.AddRangeAsync(relations);
            await dbContext.SaveChangesAsync();
            var arrangeDbContext = _fixture.CreateDbContext(true);
            UseCase.UpdateGenre updateGenre = new UseCase.UpdateGenre(
                new GenreRepository(arrangeDbContext),
                new UnitOfWork(arrangeDbContext),
                new CategoryRespository(arrangeDbContext)
                );

            var input = new UpdateGenreInput(
                targetGenre.Id,
                _fixture.GetValidGenreName(),
                !targetGenre.IsActive,
                new List<Guid>()
                );

            var output = await updateGenre.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(targetGenre.Id);
            output.Name.Should().Be(input.Name);
            output.IsActive.Should().Be((bool)input.IsActive!);
            output.Categories.Should().HaveCount(0);
            var relatedCategoryIdsFromOutput = output.Categories
                .Select(relationCategory => relationCategory.Id).ToList();
            relatedCategoryIdsFromOutput.Should().BeEquivalentTo(new List<Guid>());
            var assertDbContext = _fixture.CreateDbContext(true);
            var genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
            genreFromDb.Should().NotBeNull();
            genreFromDb!.Id.Should().Be(targetGenre.Id);
            genreFromDb!.Name.Should().Be(output.Name);
            genreFromDb.IsActive.Should().Be(output.IsActive);
            var relatedCategoryIdsFromDb = await assertDbContext.GenresCategories.AsNoTracking()
                .Where(relation => relation.GenreId == input.Id)
                .Select(relation => relation.CategoryId).ToListAsync();
            relatedCategoryIdsFromDb.Should().BeEquivalentTo(new List<Guid>());
        }
    }
}
