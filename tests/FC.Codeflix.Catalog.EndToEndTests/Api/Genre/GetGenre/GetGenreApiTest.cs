using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Infra.Data.EF.Model;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.GetGenre
{
    [Collection(nameof(GetGenreApiTestFixture))]
    public class GetGenreApiTest
    {
        private readonly GetGenreApiTestFixture _fixture;
        public GetGenreApiTest(GetGenreApiTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = (nameof(GetGenre)))]
        [Trait("EndToEnd/API", "Genre/GetGenre - EndPoints")]
        public async Task GetGenre()
        {
            var exampleGenres = _fixture.GetExampleListGenres();
            var targetGenre = exampleGenres[5];
            await _fixture.Persistence.InsertList(exampleGenres);

            var (response, output) = await _fixture.ApiClient
                .Get<ApiResponse<GenreModelOutput>>(
                $"/genres/{targetGenre.Id}");

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Data.Id.Should().Be(targetGenre.Id);
            output.Data.Name.Should().Be(targetGenre.Name);
            output.Data.IsActive.Should().Be(targetGenre.IsActive);
        }

        [Fact(DisplayName = (nameof(GenreNotFound)))]
        [Trait("EndToEnd/API", "Genre/GetGenre - EndPoints")]
        public async Task GenreNotFound()
        {
            var exampleGenres = _fixture.GetExampleListGenres();
            var randomGuid = Guid.NewGuid();
            await _fixture.Persistence.InsertList(exampleGenres);

            var (response, output) = await _fixture.ApiClient
                .Get<ProblemDetails>(
                $"/genres/{randomGuid}");

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
            output.Should().NotBeNull();
            output!.Type.Should().Be("NotFound");
            output.Detail.Should().Be($"Genre '{randomGuid}' not found.");
        }

        [Fact(DisplayName = (nameof(GetGenreWithRelations)))]
        [Trait("EndToEnd/API", "Genre/GetGenre - EndPoints")]
        public async Task GetGenreWithRelations()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleGenres = _fixture.GetExampleListGenres();
            var targetGenre = exampleGenres[5];
            var exampleCategories = _fixture.GetExampleCategoryList();
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

            var (response, output) = await _fixture.ApiClient
                .Get<ApiResponse<GenreModelOutput>>(
                $"/genres/{targetGenre.Id}");

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Data.Id.Should().Be(targetGenre.Id);
            output.Data.Name.Should().Be(targetGenre.Name);
            output.Data.IsActive.Should().Be(targetGenre.IsActive);
            var relatedCategoriesIds = output.Data.Categories.Select(
                relation => relation.Id).ToList();
            relatedCategoriesIds.Should().BeEquivalentTo(targetGenre.Categories);
        }
    }
}
