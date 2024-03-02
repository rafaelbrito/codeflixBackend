using FluentAssertions;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.Net;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Microsoft.AspNetCore.Mvc;
using FC.Codeflix.Catalog.Infra.Data.EF.Model;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.DeleteGenre
{
    [Collection(nameof(DeleteGenreApiTestFixture))] 
    public class DeleteGenreApiTest
    {
        private readonly DeleteGenreApiTestFixture _fixture;
        public DeleteGenreApiTest(DeleteGenreApiTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(DeleteGenre))]
        [Trait("EndToEnd/API", "Genre/DeleteGenre - EndPoints")]
        public async Task DeleteGenre()
        {
            var genresExampleList = _fixture.GetExampleListGenres(10);
            var targetGenre = genresExampleList[5];
            await _fixture.Persistence.InsertList(genresExampleList);

            var (response, output) = await _fixture.ApiClient.Delete<object>($"/genres/{targetGenre.Id}");

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
            output.Should().BeNull();
            DomainEntity.Genre? genre = await _fixture.Persistence
                .GetById(targetGenre.Id);
            genre.Should().BeNull();
        }

        [Fact(DisplayName = nameof(NotFound))]
        [Trait("EndToEnd/API", "Genre/DeleteGenre - EndPoints")]
        public async Task NotFound()
        {
            var genresExampleList = _fixture.GetExampleListGenres(10);
            var randomGuid = Guid.NewGuid();
            await _fixture.Persistence.InsertList(genresExampleList);

            var (response, output) = await _fixture.ApiClient.Delete<ProblemDetails>($"/genres/{randomGuid}");

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
            output.Should().NotBeNull();
            output!.Type.Should().Be("NotFound");
            output.Detail.Should().Be($"Genre '{randomGuid}' not found.");
        }

        [Fact(DisplayName = nameof(DeleteGenreWhithRelations))]
        [Trait("EndToEnd/API", "Genre/DeleteGenre - EndPoints")]
        public async Task DeleteGenreWhithRelations()
        {
            var exampleGenres = _fixture.GetExampleListGenres();
            var exampleCategories = _fixture.GetExampleCategoryList();
            var targetGenre = exampleGenres[5];
            var random = new Random();
            exampleGenres.ForEach(genre =>
            {
                int relationsCount = random.Next(2, exampleCategories.Count - 1);
                for (int i = 0; i < relationsCount; i++)
                {
                    var selectedCategoryIndex = random.Next(0, exampleCategories.Count - 1);
                    var selected = exampleCategories[selectedCategoryIndex];
                    if (!genre.Categories.Contains(selected.Id))
                        genre.AddCategory(selected.Id);
                }
            });
            var genresCategories = new List<GenresCategories>();
            exampleGenres.ForEach(genre => genre.Categories.ToList().ForEach(
                category => genresCategories.Add(new GenresCategories(category, genre.Id)))
                );

            await _fixture.CategoryPersistence.InsertList(exampleCategories);
            await _fixture.Persistence.InsertList(exampleGenres, genresCategories);

            var (response, output) = await _fixture.ApiClient.Delete<object>($"/genres/{targetGenre.Id}");

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
            output.Should().BeNull();
            DomainEntity.Genre? genre = await _fixture.Persistence
                .GetById(targetGenre.Id);
            genre.Should().BeNull();
            List<GenresCategories> relations = await _fixture.Persistence
                .GetGenresCategoriesRelationsByGenreId(targetGenre.Id);
            relations.Should().HaveCount(0);
        }
    }
}
