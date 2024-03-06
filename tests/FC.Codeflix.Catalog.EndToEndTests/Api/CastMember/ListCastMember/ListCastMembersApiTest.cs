using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.EndToEndTests.Extensions.DateTime;
using FC.Codeflix.Catalog.EndToEndTests.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.ListCastMember
{
    [Collection(nameof(CastMemberBaseFixture))]
    public class ListCastMembersApiTest : IDisposable
    {
        private readonly CastMemberBaseFixture _fixture;
        public ListCastMembersApiTest(CastMemberBaseFixture fixture)
            => _fixture = fixture;

        public void Dispose()
        {
            _fixture.ClearPersistence();
        }

        [Fact(DisplayName = (nameof(ListCastMembersAndTotal)))]
        [Trait("EndToEnd/API", "CastMember/List - EndPoints")]
        public async Task ListCastMembersAndTotal()
        {
            var exampleCastMemberList = _fixture.GetExampleCastMemberList();
            await _fixture.Persistence.InsertList(exampleCastMemberList);
            var input = new ListCastMembersInput();
            var (response, output) = await _fixture.ApiClient
                .Get<TestApiResponseList<CastMemberModelOutput>>($"/castmembers/",
                input);

            response.Should().NotBeNull();
            output.Should().NotBeNull();
            output!.Meta.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output!.Meta.Total.Should().Be(exampleCastMemberList.Count);
            output.Meta.CurrentPage.Should().Be(input.Page);
            output.Meta.PerPage.Should().Be(input.PerPage);
            output!.Data.Count.Should().Be(exampleCastMemberList.Count);
            output.Data.ToList().ForEach(outputItem =>
            {
                var exampleItem = exampleCastMemberList
                 .Find(x => x.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.Type.Should().Be(exampleItem!.Type);
                outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
            });
        }

        [Fact(DisplayName = (nameof(ListEmpty)))]
        [Trait("EndToEnd/API", "CastMember/List - EndPoints")]
        public async Task ListEmpty()
        {
            var (response, output) = await _fixture.ApiClient
             .Get<TestApiResponseList<CastMemberModelOutput>>($"/castmembers/");

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Meta.Total.Should().Be(0);
            output.Data.Should().HaveCount(0);
        }

        [Theory(DisplayName = (nameof(ListPaginated)))]
        [Trait("EndToEnd/API", "CastMembers/List - EndPoints")]
        [InlineData(10, 1, 5, 5)]
        [InlineData(10, 2, 5, 5)]
        [InlineData(7, 2, 5, 2)]
        [InlineData(7, 3, 5, 0)]
        public async Task ListPaginated(
         int quantityCategoryToGenerate,
         int page,
         int perPage,
         int expectedQuantityItems
         )
        {
            var exampleCastMembersList = _fixture.GetExampleCastMemberList(quantityCategoryToGenerate);
            await _fixture.Persistence.InsertList(exampleCastMembersList);
            var input = new ListCastMembersInput(page, perPage);

            var (response, output) = await _fixture.ApiClient
                .Get<TestApiResponseList<CastMemberModelOutput>>($"/castmembers", input);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Meta.CurrentPage.Should().Be(input.Page);
            output!.Meta.PerPage.Should().Be(input.PerPage);
            output!.Meta.Total.Should().Be(exampleCastMembersList.Count);
            output.Data.Should().HaveCount(expectedQuantityItems);
            output.Data.ToList().ForEach(outputItem =>
            {
                var exampleItem = exampleCastMembersList
                 .Find(x => x.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.Type.Should().Be(exampleItem!.Type);
                outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
            });
        }

        [Theory(DisplayName = (nameof(SearchByText)))]
        [Trait("EndToEnd/API", "Category/List - EndPoints")]
        [InlineData("Action", 1, 5, 1, 1)]
        [InlineData("Horror", 1, 5, 3, 3)]
        [InlineData("Horror", 2, 5, 0, 3)]
        [InlineData("Sci-fi", 1, 5, 4, 4)]
        [InlineData("Sci-fi", 1, 2, 2, 4)]
        [InlineData("Sci-fi", 2, 3, 1, 4)]
        [InlineData("Not Exist", 1, 3, 0, 0)]
        [InlineData("Robots", 1, 5, 2, 2)]
        public async Task SearchByText(
           string search,
           int page,
           int perPage,
           int expectedQuantityItemsReturned,
           int expectedQuantityTotalItems
           )
        {
            var castMembersNameList = new List<string>
            {
                "Action",
                "Horror",
                 "Horror - Robots",
                "Horror - Based on Real Facts",
                "Drama",
                "Sci-fi IA",
                "Sci-fi Space",
                "Sci-fi Robots",
                "Sci-fi Future"
            };
            var exampleCastMembersList = _fixture.GetExampleCastMemberListWithNames(castMembersNameList);
            await _fixture.Persistence.InsertList(exampleCastMembersList);
            var input = new ListCastMembersInput(page, perPage, search);

            var (response, output) = await _fixture.ApiClient
                .Get<TestApiResponseList<CastMemberModelOutput>>($"/castmembers", input);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Meta.CurrentPage.Should().Be(input.Page);
            output!.Meta.PerPage.Should().Be(input.PerPage);
            output!.Meta.Total.Should().Be(expectedQuantityTotalItems);
            output.Data.Should().HaveCount(expectedQuantityItemsReturned);
            output.Data.ToList().ForEach(outputItem =>
            {
                var exampleItem = exampleCastMembersList
                 .Find(x => x.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.Type.Should().Be(exampleItem!.Type);
                outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
            });
        }

        [Theory(DisplayName = (nameof(ListOrdered)))]
        [Trait("EndToEnd/API", "Category/List - EndPoints")]
        [InlineData("name", "asc")]
        [InlineData("name", "desc")]
        [InlineData("id", "asc")]
        [InlineData("id", "desc")]
        [InlineData("createdAt", "asc")]
        [InlineData("createdAt", "desc")]
        [InlineData("", "desc")]
        public async Task ListOrdered(
            string orderBy, string order
            )
        {
            var exampleCastMembersList = _fixture.GetExampleCastMemberList();
            await _fixture.Persistence.InsertList(exampleCastMembersList);
            var inputOrder = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
            var input = new ListCategoriesInput(page: 1, perPage: 20, sort: orderBy, dir: inputOrder);

            var (response, output) = await _fixture.ApiClient
                .Get<TestApiResponseList<CastMemberModelOutput>>($"/castmembers", input);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Meta.CurrentPage.Should().Be(input.Page);
            output!.Meta.PerPage.Should().Be(input.PerPage);
            output!.Meta.Total.Should().Be(exampleCastMembersList.Count);
            output.Data.Should().HaveCount(exampleCastMembersList.Count);
            output.Data.ToList().ForEach(outputItem =>
            {
                var exampleItem = exampleCastMembersList
                 .Find(x => x.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.Type.Should().Be(exampleItem!.Type);
                outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
            });
        }
    }
}
