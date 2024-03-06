using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.Codeflix.Catalog.EndToEndTests.Extensions.DateTime;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.CreateCastMember
{
    [Collection(nameof(CastMemberBaseFixture))]
    public class CreateCastMemberApiTest
    {
        private readonly CastMemberBaseFixture _fixture;
        public CreateCastMemberApiTest(CastMemberBaseFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = (nameof(Create)))]
        [Trait("EndToEnd/API", "CastMember/CreateCastMember - EndPoints")]
        public async Task Create()
        {
            var input = _fixture.GetExampleCastMember();
            var (response, output) = await _fixture.ApiClient
                .Post<ApiResponse<CastMemberModelOutput>>(
                $"/castmembers/",
                input
                );

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.Created);
            output.Should().NotBeNull();
            output!.Data.Id.Should().NotBeEmpty();
            output.Data.Name.Should().Be(input.Name);
            output.Data.Type.Should().Be(input.Type);
            output.Data.CreatedAt.Should().NotBeSameDateAs(default);
            var dbContext = await _fixture.Persistence.GetById(output.Data.Id);
            dbContext.Should().NotBeNull();
            dbContext!.Id.Should().NotBeEmpty();
            dbContext.Name.Should().Be(input.Name);
            dbContext.Type.Should().Be(input.Type);
            dbContext.CreatedAt.Should().NotBeSameDateAs(default);
        }

        [Theory(DisplayName = (nameof(ThrowWhenInvalidName)))]
        [Trait("EndToEnd/API", "CastMember/CreateCastMember - EndPoints")]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ThrowWhenInvalidName(string? name)
        {
            var input = new CreateCastMemberInput(name!, _fixture.GetRandomCastMemberType());
            var (response, output) = await _fixture.ApiClient
                .Post<ProblemDetails>(
                $"/castmembers/",
                input
                );

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
            output.Should().NotBeNull();
            output!.Title.Should().Be("One or more validation errors ocurred");
            output.Detail.Should().Be("Name should not be null or empty");
        }
    }
}
