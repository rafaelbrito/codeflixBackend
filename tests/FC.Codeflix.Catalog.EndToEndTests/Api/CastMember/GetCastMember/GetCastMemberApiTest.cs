using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.EndToEndTests.Extensions.DateTime;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.GetCastMember
{
    [Collection(nameof(CastMemberBaseFixture))]
    public class GetCastMemberApiTest:IDisposable
    {
        private readonly CastMemberBaseFixture _fixture;
        public GetCastMemberApiTest(CastMemberBaseFixture fixture)
            => _fixture = fixture;

        public void Dispose()
        {
            _fixture.ClearPersistence();
        }

        [Fact(DisplayName = (nameof(Get)))]
        [Trait("EndToEnd/API", "CastMember/GetCastMember - EndPoints")]
        public async Task Get()
        {
            var exampleCastMemberList = _fixture.GetExampleCastMemberList(20);
            await _fixture.Persistence.InsertList(exampleCastMemberList);
            var exampleCastMember = exampleCastMemberList[10];
            var (response, output) = await _fixture.ApiClient.Get<ApiResponse<CastMemberModelOutput>>(
                $"/castmembers/{exampleCastMember.Id}");

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Data.Should().NotBeNull();
            output!.Data.Id.Should().Be(exampleCastMember.Id);
            output.Data.Name.Should().Be(exampleCastMember.Name);
            output.Data.CreatedAt.TrimMillisseconds().Should().Be(exampleCastMember.CreatedAt.TrimMillisseconds());
        }

        [Fact(DisplayName = (nameof(ThrowWhenNotFound)))]
        [Trait("EndToEnd/API", "CastMember/GetCastMember - EndPoints")]
        public async Task ThrowWhenNotFound()
        {
            var exampleCastMemberList = _fixture.GetExampleCastMemberList(20);
            await _fixture.Persistence.InsertList(exampleCastMemberList);
            var randomGuid = Guid.NewGuid();
            var (response, output) = await _fixture.ApiClient.Get<ProblemDetails>(
                $"/castmembers/{randomGuid}");

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
            output.Should().NotBeNull();
            output!.Status.Should().Be((int)StatusCodes.Status404NotFound);
            output.Title.Should().Be("Not Found");
            output.Detail.Should().Be($"CastMember '{randomGuid}' not found.");
            output.Type.Should().Be("NotFound");
        }
    }
}
