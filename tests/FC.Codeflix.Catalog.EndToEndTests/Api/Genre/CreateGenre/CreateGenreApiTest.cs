using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.CreateGenre
{
    [Collection(nameof(CreateGenreApiTestFixture))]
    public class CreateGenreApiTest
    {
        private readonly CreateGenreApiTestFixture _fixture;
        public CreateGenreApiTest(CreateGenreApiTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(CreateGenre))]
        [Trait("EndToEnd/API", "Category/Create - EndPoints")]
        public async Task CreateGenre()
        {
            var input = _fixture.GetExampleInput();
            var (response, output) = await _fixture
                .ApiClient.Post<ApiResponse<GenreModelOutput>>(
                "/genres",
                input
                );

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.Created);
            output.Should().NotBeNull();
            output!.Data.Should().NotBeNull();
            output!.Data.Id.Should().NotBeEmpty();
            output.Data.Name.Should().Be(input.Name);
            output.Data.IsActive.Should().Be(input.IsActive);
            output.Data.CreatedAt.Should().NotBeSameDateAs(default);
            output.Data.Categories.Should().HaveCount(0);
            var dbGenre = await _fixture.Persistence.GetById(output.Data.Id);
            dbGenre.Should().NotBeNull();
            dbGenre!.Name.Should().Be(input.Name);
            dbGenre.IsActive.Should().Be(input.IsActive);
        }

        [Fact(DisplayName = nameof(CreateGenreWithRelations))]
        [Trait("EndToEnd/API", "Category/Create - EndPoints")]
        public async Task CreateGenreWithRelations()
        {
            var exampleCategories = _fixture.GetExampleCategoryList();
            await _fixture.CategoryPersistence.InsertList(exampleCategories);
            var relatedCategories = exampleCategories.Skip(3).Take(3).Select(x => x.Id).ToList();
            var input = _fixture.GetExampleInput(relatedCategories);

            var (response, output) = await _fixture
                .ApiClient.Post<ApiResponse<GenreModelOutput>>(
                "/genres",
                input
                );

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.Created);
            output.Should().NotBeNull();
            output!.Data.Should().NotBeNull();
            output!.Data.Id.Should().NotBeEmpty();
            output.Data.Name.Should().Be(input.Name);
            output.Data.IsActive.Should().Be(input.IsActive);
            output.Data.CreatedAt.Should().NotBeSameDateAs(default);
            output.Data.Categories.Should().HaveCount(relatedCategories.Count);
            var outputRelatedCategoryId = output.Data.Categories.Select(x => x.Id).ToList();
            outputRelatedCategoryId.Should().BeEquivalentTo(relatedCategories);
            var dbGenre = await _fixture.Persistence.GetById(output.Data.Id);
            dbGenre.Should().NotBeNull();
            dbGenre!.Name.Should().Be(input.Name);
            dbGenre.IsActive.Should().Be(input.IsActive);
            var relationsFromDb = await _fixture.Persistence
                .GetGenresCategoriesRelationsByGenreId(output.Data.Id);
            relationsFromDb.Should().NotBeNull();
            relationsFromDb!.Should().HaveCount(relatedCategories.Count);
            var relatedCategoriesIdsFromDb = relationsFromDb.Select(x => x.CategoryId).ToList();
            relatedCategoriesIdsFromDb.Should().BeEquivalentTo(relatedCategories);
        }

        [Fact(DisplayName = nameof(ErrorWithInvalidRelations))]
        [Trait("EndToEnd/API", "Category/Create - EndPoints")]
        public async Task ErrorWithInvalidRelations()
        {
            var exampleCategories = _fixture.GetExampleCategoryList();
            await _fixture.CategoryPersistence.InsertList(exampleCategories);
            var relatedCategories = exampleCategories.Skip(3).Take(3).Select(x => x.Id).ToList();
            var input = _fixture.GetExampleInput(relatedCategories);
            var randomGuid = Guid.NewGuid();
            input.CategoriesIds!.Add(randomGuid);

            var (response, output) = await _fixture
                .ApiClient.Post<ProblemDetails>(
                "/genres",
                input
                );

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
            output.Should().NotBeNull();
            output!.Type.Should().Be("RelatedAggregate");
            output.Detail.Should().Be($"Related category id (or ids) not found: '{randomGuid}'");
        }
    }
}
