using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.DeleteCastMember
{
    [Collection(nameof(CastMemberBaseFixture))]
    public class DeleteCastMemberApiTest : IDisposable
    {
        private readonly CastMemberBaseFixture _fixture;
        public DeleteCastMemberApiTest(CastMemberBaseFixture fixture)
            => _fixture = fixture;

        public void Dispose()
        {
            _fixture.ClearPersistence();
        }

        [Fact(DisplayName = (nameof(Delete)))]
        [Trait("EndToEnd/API", "CastMember/DeleteCastMember - EndPoints")]
        public async Task Delete()
        {
            var exampleCastMemberList = _fixture.GetExampleCastMemberList(20);
            await _fixture.Persistence.InsertList(exampleCastMemberList);
            var exampleCastMember = exampleCastMemberList[10];
            var (response, output) = await _fixture.ApiClient.Delete<object>(
                $"/castmembers/{exampleCastMember.Id}");

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
            output.Should().BeNull();
            var persistenceCategory = await _fixture.Persistence
                .GetById(exampleCastMember.Id);
            persistenceCategory.Should().BeNull();
        }

        [Fact(DisplayName = (nameof(ThrowWhenNotFound)))]
        [Trait("EndToEnd/API", "CastMember/DeleteCastMember - EndPoints")]
        public async Task ThrowWhenNotFound()
        {
            var exampleCastMemberList = _fixture.GetExampleCastMemberList(20);
            await _fixture.Persistence.InsertList(exampleCastMemberList);
            var randomGuid = Guid.NewGuid();
            var (response, output) = await _fixture.ApiClient.Delete<ProblemDetails>(
                $"/castmembers/{randomGuid}");

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
            output.Should().NotBeNull();
            output!.Title.Should().Be("Not Found");
            output.Detail.Should().Be($"CastMember '{randomGuid}' not found.");
            output.Type.Should().Be("NotFound");
            output.Status.Should().Be((int)StatusCodes.Status404NotFound);
        }
    }
}
