using FC.Codeflix.Catalog.Infra.Data.EF;
using FluentAssertions;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.ListCategories
{
    [Collection(nameof(ListCategoriesTestFixture))]
    public class ListCategoriesTest
    {
        private readonly ListCategoriesTestFixture _fixture;
        public ListCategoriesTest(ListCategoriesTestFixture fixture)
         => _fixture = fixture;

        [Fact(DisplayName = (nameof(SearchReturnsListAndTotal)))]
        [Trait("Integration/Application", "ListCategories - Use Cases")]
        public async Task SearchReturnsListAndTotal()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleCategoryList = _fixture.GetExampleCategoryList(10);
            await dbContext.AddRangeAsync(exampleCategoryList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var categoryRespository = new CategoryRespository(dbContext);
            var searchInput = new ListCategoriesInput(1, 20);
            var useCase = new UseCase.ListCategories(categoryRespository);

            var output = await useCase.Handle(searchInput, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.Page.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(exampleCategoryList.Count);
            output.Items.Should().HaveCount(exampleCategoryList.Count);
            foreach (CategoryModelOutput outputItem in output.Items)
            {
                var exampleItem = exampleCategoryList.Find(
                    category => category.Id == outputItem.Id
                    );
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.Description.Should().Be(exampleItem!.Description);
                outputItem.IsActive.Should().Be(exampleItem!.IsActive);
                outputItem.CreatedAt.Should().Be(exampleItem!.CreatedAt);
            }
        }

        [Fact(DisplayName = (nameof(SearchReturnsEmptyWhenEmpty)))]
        [Trait("Integration/Application", "ListCategories - Use Cases")]
        public async Task SearchReturnsEmptyWhenEmpty()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var categoryRespository = new CategoryRespository(dbContext);
            var searchInput = new ListCategoriesInput(1, 20);
            var useCase = new UseCase.ListCategories(categoryRespository);

            var output = await useCase.Handle(searchInput, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.Page.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(0);
            output.Items.Should().HaveCount(0);
        }

        [Theory(DisplayName = (nameof(SearchReturnsPagineted)))]
        [Trait("Integration/Application", "ListCategories - Use Cases")]
        [InlineData(10, 1, 5, 5)]
        [InlineData(10, 2, 5, 5)]
        [InlineData(7, 2, 5, 2)]
        [InlineData(7, 3, 5, 0)]
        public async Task SearchReturnsPagineted(
         int quantityCategoryToGenerate,
         int page,
         int perPage,
         int expectedQuantityItems
         )
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleCategoryList = _fixture.GetExampleCategoryList(quantityCategoryToGenerate);
            await dbContext.AddRangeAsync(exampleCategoryList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var categoryRespository = new CategoryRespository(dbContext);
            var searchInput = new ListCategoriesInput(page, perPage);
            var useCase = new UseCase.ListCategories(categoryRespository);

            var output = await useCase.Handle(searchInput, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.Page.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(exampleCategoryList.Count);
            output.Items.Should().HaveCount(expectedQuantityItems);
            foreach (CategoryModelOutput outputItem in output.Items)
            {
                var exampleItem = exampleCategoryList.Find(
                    category => category.Id == outputItem.Id
                    );
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.Description.Should().Be(exampleItem!.Description);
                outputItem.IsActive.Should().Be(exampleItem!.IsActive);
                outputItem.CreatedAt.Should().Be(exampleItem!.CreatedAt);
            }
        }

        [Theory(DisplayName = (nameof(SearchByText)))]
        [Trait("Integration/Application", "ListCategories - Use Cases")]
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
    int expectedQuantityTotalItems)
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

            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleCategoryList = _fixture.GetExampleCategoriesListWithNames(categoryNameList);
            await dbContext.AddRangeAsync(exampleCategoryList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var categoryRespository = new CategoryRespository(dbContext);
            var searchInput = new ListCategoriesInput(page, perPage, search);
            var useCase = new UseCase.ListCategories(categoryRespository);

            var output = await useCase.Handle(searchInput, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.Page.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(expectedQuantityTotalItems);
            output.Items.Should().HaveCount(expectedQuantityItemsReturned);
            foreach (CategoryModelOutput outputItem in output.Items)
            {
                var exampleItem = exampleCategoryList.Find(
                    category => category.Id == outputItem.Id
                    );
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.Description.Should().Be(exampleItem!.Description);
                outputItem.IsActive.Should().Be(exampleItem!.IsActive);
                outputItem.CreatedAt.Should().Be(exampleItem!.CreatedAt);
            }
        }

        [Theory(DisplayName = (nameof(SearchOrdered)))]
        [Trait("Integration/Application", "ListCategories - Use Cases")]
        [InlineData("name", "asc")]
        [InlineData("name", "desc")]
        [InlineData("id", "asc")]
        [InlineData("id", "desc")]
        [InlineData("createdAt", "asc")]
        [InlineData("createdAt", "desc")]
        [InlineData("", "desc")]
        public async Task SearchOrdered(string orderBy, string order)
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleCategoryList = _fixture.GetExampleCategoryList(10);
            await dbContext.AddRangeAsync(exampleCategoryList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var categoryRespository = new CategoryRespository(dbContext);
            var useCaseOrder = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
            var searchInput = new ListCategoriesInput(1, 20, "", orderBy, useCaseOrder);
            var useCase = new UseCase.ListCategories(categoryRespository);

            var output = await useCase.Handle(searchInput, CancellationToken.None);
            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.Page.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(exampleCategoryList.Count);
            output.Items.Should().HaveCount(exampleCategoryList.Count);

            var expectedOrderedList = _fixture.CloneCategoryListOrdered(exampleCategoryList, orderBy, useCaseOrder);

            for (int i = 0; i < expectedOrderedList.Count; i++)
            {
                var expectedItem = expectedOrderedList[i];
                var outputItem = output.Items[i];
                expectedItem.Should().NotBeNull();
                outputItem.Should().NotBeNull();
                outputItem.Name.Should().Be(expectedItem.Name);
                outputItem.Id.Should().Be(expectedItem.Id);
                outputItem.Description.Should().Be(expectedItem.Description);
                outputItem.IsActive.Should().Be(expectedItem.IsActive);
                outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
            }

        }
    }
}

