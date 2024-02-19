using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.DeleteCategory
{
    [Collection(nameof(DeleteCategoryApiTestFixture))]
    public class DeleteCategoryApiTest: IDisposable
    {
        private readonly DeleteCategoryApiTestFixture _fixture;
        public DeleteCategoryApiTest(DeleteCategoryApiTestFixture fixture)
                 => _fixture = fixture;

        [Fact(DisplayName = nameof(DeleteCategory))]
        [Trait("EndToEnd/API", "Category/Delete - EndPoints")]
        public async Task DeleteCategory()
        {
            var exampleCategoryList = _fixture.GetExampleCategoryList(20);
            await _fixture.Persistence.InsertList(exampleCategoryList);
            var exampleCategory = exampleCategoryList[10];
            var (response, output) = await _fixture.ApiClient.Delete<object>(
                $"/categories/{exampleCategory.Id}");

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
            output.Should().BeNull();
            var persistenceCategory = await _fixture.Persistence
                .GetById(exampleCategory.Id);
            persistenceCategory.Should().BeNull();
        }

        [Fact(DisplayName = nameof(ErrorWhenNotFoud))]
        [Trait("EndToEnd/API", "Category/Delete - EndPoints")]
        public async Task ErrorWhenNotFoud()
        {
            var exampleCategoryList = _fixture.GetExampleCategoryList(20);
            await _fixture.Persistence.InsertList(exampleCategoryList);
            var randomGuid = Guid.NewGuid();
            var (response, output) = await _fixture.ApiClient.Delete<ProblemDetails>(
                $"/categories/{randomGuid}");

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
            output.Should().NotBeNull();
            output!.Title.Should().Be("Not Found");
            output.Detail.Should().Be($"Category '{randomGuid}' not found.");
            output.Type.Should().Be("NotFound");
            output.Status.Should().Be((int)StatusCodes.Status404NotFound);
        }

        public void Dispose()
         => _fixture.ClearPersistence();
    }
}
