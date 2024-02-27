using FC.Codeflix.Catalog.Infra.Data.EF;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using Xunit;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CategoryRespository
{
    [Collection(nameof(CategoryRepositoryTestFixture))]
    public class CategoryRepositoryTest
    {
        private readonly CategoryRepositoryTestFixture _fixture;
        public CategoryRepositoryTest(CategoryRepositoryTestFixture fixture)
        => _fixture = fixture;

        [Fact(DisplayName = (nameof(Insert)))]
        [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
        public async Task Insert()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleCategory = _fixture.GetExampleCategory();
            var categoryRespository = new Repository.CategoryRespository(dbContext);

            await categoryRespository.Insert(exampleCategory, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var dbCategory = await (_fixture.CreateDbContext(true)).Categories.FindAsync(exampleCategory.Id);
            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(exampleCategory.Name);
            dbCategory.Description.Should().Be(exampleCategory.Description);
            dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
            dbCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
        }

        [Fact(DisplayName = (nameof(Get)))]
        [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
        public async Task Get()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleCategory = _fixture.GetExampleCategory();
            var exampleCategoryList = _fixture.GetExampleCategoryList(15);
            exampleCategoryList.Add(exampleCategory);
            await dbContext.AddRangeAsync(exampleCategoryList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var categoryRespository = new Repository.CategoryRespository(_fixture.CreateDbContext(true));

            var dbCategory = await categoryRespository.Get(exampleCategory.Id, CancellationToken.None);

            dbCategory.Should().NotBeNull();
            dbCategory.Id.Should().Be(exampleCategory.Id);
            dbCategory!.Name.Should().Be(exampleCategory.Name);
            dbCategory.Description.Should().Be(exampleCategory.Description);
            dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
            dbCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
        }

        [Fact(DisplayName = (nameof(GetThrowNotFound)))]
        [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
        public async Task GetThrowNotFound()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleCategoryId = Guid.NewGuid();
            await dbContext.AddRangeAsync(_fixture.GetExampleCategory());
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var categoryRespository = new Repository.CategoryRespository(dbContext);

            var task = async () => await categoryRespository.Get(exampleCategoryId, CancellationToken.None);

            await task.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Category '{exampleCategoryId}' not found.");

        }

        [Fact(DisplayName = (nameof(Update)))]
        [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
        public async Task Update()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleCategory = _fixture.GetExampleCategory();
            var newCategoryValue = _fixture.GetExampleCategory();
            var exampleCategoryList = _fixture.GetExampleCategoryList(15);
            exampleCategoryList.Add(exampleCategory);
            await dbContext.AddRangeAsync(exampleCategoryList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var categoryRespository = new Repository.CategoryRespository(dbContext);


            exampleCategory.Update(newCategoryValue.Name, newCategoryValue.Description);
            await categoryRespository.Update(exampleCategory, CancellationToken.None);
            await dbContext.SaveChangesAsync();

            var dbCategory = await (_fixture.CreateDbContext(true)).Categories.FindAsync(exampleCategory.Id);
            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(exampleCategory.Name);
            dbCategory.Description.Should().Be(exampleCategory.Description);
            dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
            dbCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
        }

        [Fact(DisplayName = (nameof(Delete)))]
        [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
        public async Task Delete()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleCategory = _fixture.GetExampleCategory();
            var exampleCategoryList = _fixture.GetExampleCategoryList(15);
            exampleCategoryList.Add(exampleCategory);
            await dbContext.AddRangeAsync(exampleCategoryList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var categoryRespository = new Repository.CategoryRespository(dbContext);


            await categoryRespository.Delete(exampleCategory, CancellationToken.None);
            await dbContext.SaveChangesAsync();

            var dbCategory = await (_fixture.CreateDbContext(true))
                .Categories.FindAsync(exampleCategory.Id);
            dbCategory.Should().BeNull();

        }

        [Fact(DisplayName = (nameof(SearchReturnsListAndTotal)))]
        [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
        public async Task SearchReturnsListAndTotal()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleCategoryList = _fixture.GetExampleCategoryList(15);
            await dbContext.AddRangeAsync(exampleCategoryList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var categoryRespository = new Repository.CategoryRespository(dbContext);
            var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

            var output = await categoryRespository.Search(searchInput, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.CurrentPage.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(exampleCategoryList.Count);
            output.Items.Should().HaveCount(exampleCategoryList.Count);
            foreach (Category outputItem in output.Items)
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

        [Fact(DisplayName = (nameof(SearchReturnsEmptyWhenPersistenceIsEmpty)))]
        [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
        public async Task SearchReturnsEmptyWhenPersistenceIsEmpty()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var categoryRespository = new Repository.CategoryRespository(dbContext);
            var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

            var output = await categoryRespository.Search(searchInput, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.CurrentPage.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(0);
            output.Items.Should().HaveCount(0);
        }

        [Theory(DisplayName = (nameof(SearchReturnsPagineted)))]
        [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
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
            var categoryRespository = new Repository.CategoryRespository(dbContext);
            var searchInput = new SearchInput(page, perPage, "", "", SearchOrder.Asc);

            var output = await categoryRespository.Search(searchInput, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.CurrentPage.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(quantityCategoryToGenerate);
            output.Items.Should().HaveCount(expectedQuantityItems);
            foreach (Category outputItem in output.Items)
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
        [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
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
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleCategoryList = _fixture
                .GetExampleCategoriesListWithNames(new List<string>() {
                "Action",
                "Horror",
                 "Horror - Robots",
                "Horror - Based on Real Facts",
                "Drama",
                "Sci-fi IA",
                "Sci-fi Space",
                "Sci-fi Robots",
                "Sci-fi Future"
                });
            await dbContext.AddRangeAsync(exampleCategoryList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var categoryRespository = new Repository.CategoryRespository(dbContext);
            var searchInput = new SearchInput(page, perPage, search, "", SearchOrder.Asc);

            var output = await categoryRespository.Search(searchInput, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.CurrentPage.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(expectedQuantityTotalItems);
            output.Items.Should().HaveCount(expectedQuantityItemsReturned);
            foreach (Category outputItem in output.Items)
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
        [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
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
            var categoryRespository = new Repository.CategoryRespository(dbContext);
            var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
            var searchInput = new SearchInput(1, 20, "", orderBy.ToLower(), searchOrder);

            var output = await categoryRespository.Search(searchInput, CancellationToken.None);

            var expectedOrderedList = _fixture.CloneCategoryListOrdered(exampleCategoryList, orderBy.ToLower(), searchOrder);


            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.CurrentPage.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(exampleCategoryList.Count);
            output.Items.Should().HaveCount(exampleCategoryList.Count);
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

        [Fact(DisplayName = (nameof(ListByIds)))]
        [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
        public async Task ListByIds()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleCategoryList = _fixture.GetExampleCategoryList(15);
            var categoryIdsToGet = Enumerable.Range(1, 3).Select(_ => {
                var indexToget = (new Random().Next(0, exampleCategoryList.Count - 1));
                return exampleCategoryList[indexToget].Id;
            }).Distinct().ToList();

            await dbContext.AddRangeAsync(exampleCategoryList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var categoryRespository = new Repository.CategoryRespository(dbContext);

            var categoriesList = await categoryRespository.GetListByIds(categoryIdsToGet, CancellationToken.None);

            categoriesList.Should().NotBeNull();
            categoriesList.Should().HaveCount(categoryIdsToGet.Count);
            foreach (Category outputItem in categoriesList)
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
    }
}
