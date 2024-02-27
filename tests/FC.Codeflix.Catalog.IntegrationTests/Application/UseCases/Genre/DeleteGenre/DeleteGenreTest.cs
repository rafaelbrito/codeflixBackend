using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;
using Xunit;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using FC.Codeflix.Catalog.Infra.Data.EF.Model;
using FC.Codeflix.Catalog.Application.Exceptions;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.DeleteGenre
{
    [Collection(nameof(DeleteGenreTestFixture))]
    public class DeleteGenreTest
    {
        private readonly DeleteGenreTestFixture _fixture;
        public DeleteGenreTest(DeleteGenreTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(DeleteGenre))]
        [Trait("Integration/Application", "DeleteGenre - Use Cases")]
        public async Task DeleteGenre()
        {
            var dbContext = _fixture.CreateDbContext();
            var genresExampleList = _fixture.GetExampleListGenres(10);
            var targetGenre = genresExampleList[5];
            await dbContext.Genres.AddRangeAsync(genresExampleList);
            await dbContext.SaveChangesAsync();
            var actDbContext = _fixture.CreateDbContext(true);
            var useCase = new UseCase.DeleteGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext));
            var input = new UseCase.DeleteGenreInput(targetGenre.Id);

            await useCase.Handle(input, CancellationToken.None);

            var assertDbContext = _fixture.CreateDbContext(true);
            var genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
            genreFromDb.Should().BeNull();
        }

        [Fact(DisplayName = nameof(DeleteGenreThrowsWhenNotFound))]
        [Trait("Integration/Application", "DeleteGenre - Use Cases")]
        public async Task DeleteGenreThrowsWhenNotFound()
        {
            var dbContext = _fixture.CreateDbContext();
            var genresExampleList = _fixture.GetExampleListGenres(10);
            var targetGenre = Guid.NewGuid();
            await dbContext.Genres.AddRangeAsync(genresExampleList);
            await dbContext.SaveChangesAsync();
            var actDbContext = _fixture.CreateDbContext(true);
            var useCase = new UseCase.DeleteGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext));
            var input = new UseCase.DeleteGenreInput(targetGenre);

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Genre '{targetGenre}' not found.");
            
        }

        [Fact(DisplayName = nameof(DeleteGenreWithRelations))]
        [Trait("Integration/Application", "DeleteGenre - Use Cases")]
        public async Task DeleteGenreWithRelations()
        {
            var dbContext = _fixture.CreateDbContext();
            var genresExampleList = _fixture.GetExampleListGenres(10);
            var exampleCategories = _fixture.GetExampleCategoryList(5);
            var targetGenre = genresExampleList[5];
            await dbContext.Categories.AddRangeAsync(exampleCategories);
            await dbContext.Genres.AddRangeAsync(genresExampleList);
            await dbContext.GenresCategories.AddRangeAsync(
                exampleCategories.Select(category => 
                    new GenresCategories(category.Id, targetGenre.Id))
                );
            await dbContext.SaveChangesAsync();
            var actDbContext = _fixture.CreateDbContext(true);
            var useCase = new UseCase.DeleteGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext));
            var input = new UseCase.DeleteGenreInput(targetGenre.Id);

            await useCase.Handle(input, CancellationToken.None);

            var assertDbContext = _fixture.CreateDbContext(true);
            var genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
            genreFromDb.Should().BeNull();
            var relations = await assertDbContext.GenresCategories.AsNoTracking()
                .Where(relation => relation.GenreId == targetGenre.Id).ToListAsync();
            relations.Should().HaveCount(0);
        }
    }
}
