using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.EndToEndTests.Extensions.DateTime;
using FC.Codeflix.Catalog.EndToEndTests.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;
using Xunit;
using Xunit.Abstractions;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.ListCategories
{
    
    [Collection(nameof(ListCategoriesApiTestFixture))]
    public class ListCategoriesApiTest : IDisposable
    {
        private readonly ListCategoriesApiTestFixture _fixture;
        private readonly ITestOutputHelper _output;

        public ListCategoriesApiTest(ListCategoriesApiTestFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        [Fact(DisplayName = (nameof(ListCategoriesAndTotalByDefault)))]
        [Trait("EndToEnd/API", "Category/List - EndPoints")]
        public async Task ListCategoriesAndTotalByDefault()
        {
            var defaultPerPage = 15;
            var exampleCategoryList = _fixture.GetExampleCategoryList(20);
            await _fixture.Persistence.InsertList(exampleCategoryList);
            var (response, output) = await _fixture.ApiClient
                .Get<TestApiResponseList<CategoryModelOutput>>($"/categories/");

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Data.Should().NotBeNull();
            output.Meta.Should().NotBeNull();
            output!.Meta.Total.Should().Be(exampleCategoryList.Count);
            output!.Meta.CurrentPage.Should().Be(1);
            output.Meta.PerPage.Should().Be(defaultPerPage);
            output.Data.Should().HaveCount(defaultPerPage);
            foreach (CategoryModelOutput outputItem in output.Data!)
            {
                var exampleItem = exampleCategoryList
                      .FirstOrDefault(x => x.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.Description.Should().Be(exampleItem!.Description);
                outputItem.IsActive.Should().Be(exampleItem!.IsActive);
                outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem!.CreatedAt.TrimMillisseconds());
            }
        }

        [Fact(DisplayName = (nameof(ItemsEmptyWhenPercistenceEmpty)))]
        [Trait("EndToEnd/API", "Category/List - EndPoints")]
        public async Task ItemsEmptyWhenPercistenceEmpty()
        {
            var (response, output) = await _fixture.ApiClient
                .Get<TestApiResponseList<CategoryModelOutput>>($"/categories/");

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Meta.Total.Should().Be(0);
            output.Data.Should().HaveCount(0);
        }


        [Fact(DisplayName = (nameof(ListCategoriesAndTotal)))]
        [Trait("EndToEnd/API", "Category/List - EndPoints")]
        public async Task ListCategoriesAndTotal()
        {
            var exampleCategoryList = _fixture.GetExampleCategoryList(20);
            await _fixture.Persistence.InsertList(exampleCategoryList);
            var input = new ListCategoriesInput(page: 1, perPage: 5);

            var (response, output) = await _fixture.ApiClient
                .Get<TestApiResponseList<CategoryModelOutput>>($"/categories", input);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Meta.CurrentPage.Should().Be(input.Page);
            output!.Meta.PerPage.Should().Be(input.PerPage);
            output!.Meta.Total.Should().Be(exampleCategoryList.Count);
            output.Data.Should().HaveCount(input.PerPage);
            foreach (CategoryModelOutput outputItem in output.Data!)
            {
                var exampleItem = exampleCategoryList
                      .FirstOrDefault(x => x.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.Description.Should().Be(exampleItem!.Description);
                outputItem.IsActive.Should().Be(exampleItem!.IsActive);
                outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem!.CreatedAt.TrimMillisseconds());
            }
        }

        [Theory(DisplayName = (nameof(ListPaginated)))]
        [Trait("EndToEnd/API", "Category/List - EndPoints")]
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
            var exampleCategoryList = _fixture.GetExampleCategoryList(quantityCategoryToGenerate);
            await _fixture.Persistence.InsertList(exampleCategoryList);
            var input = new ListCategoriesInput(page, perPage);

            var (response, output) = await _fixture.ApiClient
                .Get<TestApiResponseList<CategoryModelOutput>>($"/categories", input);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Meta.CurrentPage.Should().Be(input.Page);
            output!.Meta.PerPage.Should().Be(input.PerPage);
            output!.Meta.Total.Should().Be(exampleCategoryList.Count);
            output.Data.Should().HaveCount(expectedQuantityItems);
            foreach (CategoryModelOutput outputItem in output.Data!)
            {
                var exampleItem = exampleCategoryList
                      .FirstOrDefault(x => x.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.Description.Should().Be(exampleItem!.Description);
                outputItem.IsActive.Should().Be(exampleItem!.IsActive);
                outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem!.CreatedAt.TrimMillisseconds());
            }
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
            var categoryNameList = new List<string>
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
            var exampleCategoryList = _fixture.GetExampleCategoriesListWithNames(categoryNameList);
            await _fixture.Persistence.InsertList(exampleCategoryList);
            var input = new ListCategoriesInput(page, perPage, search);

            var (response, output) = await _fixture.ApiClient
                .Get<TestApiResponseList<CategoryModelOutput>>($"/categories", input);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Meta.CurrentPage.Should().Be(input.Page);
            output!.Meta.PerPage.Should().Be(input.PerPage);
            output!.Meta.Total.Should().Be(expectedQuantityTotalItems);
            output.Data.Should().HaveCount(expectedQuantityItemsReturned);
            foreach (CategoryModelOutput outputItem in output.Data!)
            {
                var exampleItem = exampleCategoryList
                      .FirstOrDefault(x => x.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.Description.Should().Be(exampleItem!.Description);
                outputItem.IsActive.Should().Be(exampleItem!.IsActive);
                outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem!.CreatedAt.TrimMillisseconds());
            }
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
            var exampleCategoryList = _fixture.GetExampleCategoryList(10);
            await _fixture.Persistence.InsertList(exampleCategoryList);
            var inputOrder = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
            var input = new ListCategoriesInput(page: 1, perPage: 20, sort: orderBy, dir: inputOrder);

            var (response, output) = await _fixture.ApiClient
                .Get<TestApiResponseList<CategoryModelOutput>>($"/categories", input);


            var expectedOrderedList = _fixture.CloneCategoryListOrdered(exampleCategoryList, orderBy, inputOrder);
            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Meta.CurrentPage.Should().Be(input.Page);
            output!.Meta.PerPage.Should().Be(input.PerPage);
            output!.Meta.Total.Should().Be(exampleCategoryList.Count);
            output.Data.Should().HaveCount(exampleCategoryList.Count);
            for (int i = 0; i < expectedOrderedList.Count; i++)
            {
                var expectedItem = expectedOrderedList[i];
                var outputItem = output.Data![i];
                expectedItem.Should().NotBeNull();
                outputItem.Should().NotBeNull();
                outputItem.Name.Should().Be(expectedItem.Name);
                outputItem.Id.Should().Be(expectedItem.Id);
                outputItem.Description.Should().Be(expectedItem.Description);
                outputItem.IsActive.Should().Be(expectedItem.IsActive);
                outputItem.CreatedAt.TrimMillisseconds().Should().Be(expectedItem.CreatedAt.TrimMillisseconds());
            }
        }

        [Theory(DisplayName = (nameof(ListOrderedDates)))]
        [Trait("EndToEnd/API", "Category/List - EndPoints")]
        [InlineData("createdAt", "asc")]
        [InlineData("createdAt", "desc")]
        public async Task ListOrderedDates(
            string orderBy, string order
            )
        {
            var exampleCategoryList = _fixture.GetExampleCategoryList(10);
            await _fixture.Persistence.InsertList(exampleCategoryList);
            var inputOrder = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
            var input = new ListCategoriesInput(page: 1, perPage: 20, sort: orderBy, dir: inputOrder);

            var (response, output) = await _fixture.ApiClient
                .Get<TestApiResponseList<CategoryModelOutput>>($"/categories", input);


            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Meta.CurrentPage.Should().Be(input.Page);
            output!.Meta.PerPage.Should().Be(input.PerPage);
            output!.Meta.Total.Should().Be(exampleCategoryList.Count);
            output.Data.Should().HaveCount(exampleCategoryList.Count);
            DateTime? lastItemDate = null;
            foreach (CategoryModelOutput outputItem in output.Data)
            {
                var exampleItem = exampleCategoryList
                      .FirstOrDefault(x => x.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.Description.Should().Be(exampleItem!.Description);
                outputItem.IsActive.Should().Be(exampleItem!.IsActive);
                outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem!.CreatedAt.TrimMillisseconds());

                if (lastItemDate != null)
                {
                    if (order == "asc")
                        Assert.True(outputItem.CreatedAt >= lastItemDate);
                    else
                        Assert.True(outputItem.CreatedAt <= lastItemDate);
                }
                lastItemDate = outputItem.CreatedAt;
            }
        }











        public void Dispose()
           => _fixture.ClearPersistence();
    }

}
