using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using FC.Codeflix.Catalog.EndToEndTests.Extensions.DateTime;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.UpdateCastMember
{
    [Collection(nameof(CastMemberBaseFixture))]
    public class UpdateCastMemberApiTest
    {
        private readonly CastMemberBaseFixture _fixture;
        public UpdateCastMemberApiTest(CastMemberBaseFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = (nameof(Update)))]
        [Trait("EndToEnd/API", "CastMember/UpdateCastMember - EndPoints")]
        public async Task Update()
        {
            var exampleCastMemberList = _fixture.GetExampleCastMemberList(20);
            await _fixture.Persistence.InsertList(exampleCastMemberList);
            var exampleCastMember = exampleCastMemberList[10];
            var input = new UpdateCastMemberInput(exampleCastMember.Id, _fixture.GetValidName(), _fixture.GetRandomCastMemberType());
            var (response, output) = await _fixture.ApiClient.Put<ApiResponse<CastMemberModelOutput>>(
                $"/castmembers/{exampleCastMember.Id}",
                input
                );

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Data.Should().NotBeNull();
            output!.Data.Id.Should().Be(input.Id);
            output.Data.Name.Should().Be(input.Name);

            var dbContext = await _fixture.Persistence.GetById(output.Data.Id);
            dbContext.Should().NotBeNull();
            dbContext!.Id.Should().NotBeEmpty();
            dbContext.Name.Should().Be(input.Name);
            dbContext.Type.Should().Be(input.Type);
            dbContext.CreatedAt.Should().NotBeSameDateAs(default);
        }

        [Fact(DisplayName = (nameof(ThrowWhenNotFound)))]
        [Trait("EndToEnd/API", "CastMember/UpdateCastMember - EndPoints")]
        public async Task ThrowWhenNotFound()
        {
            var exampleCastMemberList = _fixture.GetExampleCastMemberList(20);
            await _fixture.Persistence.InsertList(exampleCastMemberList);
            var randomGuid = Guid.NewGuid();
            var input = _fixture.GetExampleCastMember();
            var (response, output) = await _fixture.ApiClient.Put<ProblemDetails>(
                $"/castmembers/{randomGuid}",
                input
                );

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
            output.Should().NotBeNull();
            output!.Title.Should().Be("Not Found");
            output.Type.Should().Be("NotFound");
            output.Detail.Should().Be($"CastMember '{randomGuid}' not found.");
            output.Status.Should().Be((int)StatusCodes.Status404NotFound);
        }
    }
}
