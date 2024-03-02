using FC.Codeflix.Catalog.Api.ApiModels.Category;
using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Infra.Data.EF.Model;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.UpdateGenre
{
    [Collection(nameof(UpdateGenreApiTestFixture))]
    public class UpdateGenreApiTest
    {
        private readonly UpdateGenreApiTestFixture _fixture;
        public UpdateGenreApiTest(UpdateGenreApiTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = (nameof(UpdateGenre)))]
        [Trait("EndToEnd/API", "Genre/UpdateGenre - EndPoints")]
        public async Task UpdateGenre()
        {
            var exampleGenres = _fixture.GetExampleListGenres();
            var targetGenre = exampleGenres[5];
            await _fixture.Persistence.InsertList(exampleGenres);
            var input = new UpdateGenreApiInput(_fixture.GetValidGenreName(), _fixture.GetRandomBoolean());
              
            var (response, output) = await _fixture.ApiClient
                .Put<ApiResponse<GenreModelOutput>>(
                $"/genres/{targetGenre.Id}",
                input
                );

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Data.Id.Should().Be(targetGenre.Id);
            output.Data.Name.Should().Be(input.Name);
            output.Data.IsActive.Should().Be((bool)input.IsActive!);
            var dbGenre = await _fixture.Persistence.GetById(output.Data.Id);
            dbGenre.Should().NotBeNull();
            dbGenre!.Name.Should().Be(input.Name);
            dbGenre.IsActive.Should().Be((bool)input.IsActive);
        }

        [Fact(DisplayName = (nameof(ProblemDetailsWhenNotFound)))]
        [Trait("EndToEnd/API", "Genre/UpdateGenre - EndPoints")]
        public async Task ProblemDetailsWhenNotFound()
        {
            var exampleGenres = _fixture.GetExampleListGenres();
            var randomGuid = Guid.NewGuid();
            await _fixture.Persistence.InsertList(exampleGenres);
            var input = new UpdateGenreApiInput(_fixture.GetValidGenreName(), _fixture.GetRandomBoolean());

            var (response, output) = await _fixture.ApiClient
                .Put<ProblemDetails>(
                $"/genres/{randomGuid}",
                input
                );

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
            output.Should().NotBeNull();
            output!.Title.Should().Be("Not Found");
            output!.Detail.Should().Be($"Genre '{randomGuid}' not found.");
            output.Type.Should().Be("NotFound");
            output.Status.Should().Be((int)StatusCodes.Status404NotFound);

        }

        [Fact(DisplayName = (nameof(UpdateGenreWithRelations)))]
        [Trait("EndToEnd/API", "Genre/UpdateGenre - EndPoints")]
        public async Task UpdateGenreWithRelations()
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
            int newRelationsCount = random.Next(2, exampleCategories.Count - 1);
            var newRelationsCategoriesIds = new List<Guid>();
            for (int i = 0; i < newRelationsCount; i++)
            {
                var selectedCategoryIndex = random.Next(0, exampleCategories.Count - 1);
                var selected = exampleCategories[selectedCategoryIndex];
                if (!newRelationsCategoriesIds.Contains(selected.Id))
                    newRelationsCategoriesIds.Add(selected.Id);
            }

            await _fixture.CategoryPersistence.InsertList(exampleCategories);
            await _fixture.Persistence.InsertList(exampleGenres, genresCategories);

            var input = new UpdateGenreApiInput(
                _fixture.GetValidGenreName(), 
                _fixture.GetRandomBoolean(), 
                newRelationsCategoriesIds
                );
            
            var (response, output) = await _fixture.ApiClient
                .Put<ApiResponse<GenreModelOutput>>(
                $"/genres/{targetGenre.Id}",
                input
                );

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Data.Id.Should().Be(targetGenre.Id);
            output.Data.Name.Should().Be(input.Name);
            output.Data.IsActive.Should().Be((bool)input.IsActive!);
            var relatedCategoriesIds = output.Data.Categories.Select(
                relation => relation.Id).ToList();
            relatedCategoriesIds.Should().BeEquivalentTo(newRelationsCategoriesIds);
            var dbGenre = await _fixture.Persistence.GetById(output.Data.Id);
            dbGenre.Should().NotBeNull();
            dbGenre!.Name.Should().Be(input.Name);
            dbGenre.IsActive.Should().Be((bool)input.IsActive);

            var relationsFromDb = await _fixture.Persistence
                .GetGenresCategoriesRelationsByGenreId(output.Data.Id);
            relationsFromDb.Should().NotBeNull();
            relationsFromDb!.Should().HaveCount(newRelationsCategoriesIds.Count);
            var relatedCategoriesIdsFromDb = relationsFromDb.Select(x => x.CategoryId).ToList();
            relatedCategoriesIdsFromDb.Should().BeEquivalentTo(newRelationsCategoriesIds);
        }

        [Fact(DisplayName = (nameof(UpdateGenreWithInvalidRelations)))]
        [Trait("EndToEnd/API", "Genre/UpdateGenre - EndPoints")]
        public async Task UpdateGenreWithInvalidRelations()
        {
            var exampleGenres = _fixture.GetExampleListGenres();
            var targetGenre = exampleGenres[5];
            var randomGuid = Guid.NewGuid();
            await _fixture.Persistence.InsertList(exampleGenres);
            var input = new UpdateGenreApiInput(
                _fixture.GetValidGenreName(), 
                _fixture.GetRandomBoolean(),
                new List<Guid>() { randomGuid}
                );

            var (response, output) = await _fixture.ApiClient
                .Put<ProblemDetails>(
                $"/genres/{targetGenre.Id}",
                input
                );

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
            output.Should().NotBeNull();
            output!.Type.Should().Be("RelatedAggregate");
            output.Detail.Should().Be($"Related category id (or ids) not found: '{randomGuid}'");
        }

        [Fact(DisplayName = (nameof(UpdateGenrePersistsRelations)))]
        [Trait("EndToEnd/API", "Genre/UpdateGenre - EndPoints")]
        public async Task UpdateGenrePersistsRelations()
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

            var input = new UpdateGenreApiInput(
                _fixture.GetValidGenreName(),
                _fixture.GetRandomBoolean()
                );

            var (response, output) = await _fixture.ApiClient
                .Put<ApiResponse<GenreModelOutput>>(
                $"/genres/{targetGenre.Id}",
                input
                );

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Data.Id.Should().Be(targetGenre.Id);
            output.Data.Name.Should().Be(input.Name);
            output.Data.IsActive.Should().Be((bool)input.IsActive!);
            var relatedCategoriesIds = output.Data.Categories.Select(
                relation => relation.Id).ToList();
            relatedCategoriesIds.Should().BeEquivalentTo(targetGenre.Categories);
            var dbGenre = await _fixture.Persistence.GetById(output.Data.Id);
            dbGenre.Should().NotBeNull();
            dbGenre!.Name.Should().Be(input.Name);
            dbGenre.IsActive.Should().Be((bool)input.IsActive);

            var relationsFromDb = await _fixture.Persistence
                .GetGenresCategoriesRelationsByGenreId(output.Data.Id);
            relationsFromDb.Should().NotBeNull();
            relationsFromDb!.Should().HaveCount(targetGenre.Categories.Count);
            var relatedCategoriesIdsFromDb = relationsFromDb.Select(x => x.CategoryId).ToList();
            relatedCategoriesIdsFromDb.Should().BeEquivalentTo(targetGenre.Categories);
        }
    }
}
