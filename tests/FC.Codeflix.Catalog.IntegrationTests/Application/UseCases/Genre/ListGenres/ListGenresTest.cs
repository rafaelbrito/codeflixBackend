using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Model;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenres;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.ListGenres
{
    [Collection(nameof(ListGenresTestFixture))]
    public class ListGenresTest
    {
        private readonly ListGenresTestFixture _fixture;
        public ListGenresTest(ListGenresTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = (nameof(ListGenres)))]
        [Trait("Integration/Application", "ListGenres - Use Cases")]
        public async Task ListGenres()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleGenres = _fixture.GetExampleListGenres();
            await dbContext.AddRangeAsync(exampleGenres);
            await dbContext.SaveChangesAsync();
            var actDbContext = _fixture.CreateDbContext(true);
            var useCase = new UseCase.ListGenres(new GenreRepository(actDbContext), new CategoryRespository(actDbContext));
            var input = new UseCase.ListGenresInput(1, 20);
            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().HaveCount(exampleGenres.Count);
            output.Total.Should().Be(exampleGenres.Count);
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Items.ToList().ForEach(outputItem =>
            {
                var exampleItem = exampleGenres
                    .Find(example => example.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.IsActive.Should().Be(exampleItem!.IsActive);
            });
        }

        [Fact(DisplayName = (nameof(ListReturnsEmptyWhenPersistenceIsEmpty)))]
        [Trait("Integration/Application", "ListGenres - Use Cases")]
        public async Task ListReturnsEmptyWhenPersistenceIsEmpty()
        {
            var dbContext = _fixture.CreateDbContext();
            var useCase = new UseCase.ListGenres(new GenreRepository(dbContext), new CategoryRespository(dbContext));
            var input = new UseCase.ListGenresInput(1, 20);
            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().HaveCount(0);
            output.Total.Should().Be(0);
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
        }

        [Fact(DisplayName = (nameof(ListGenresVerifyRelations)))]
        [Trait("Integration/Application", "ListGenres - Use Cases")]
        public async Task ListGenresVerifyRelations()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleGenres = _fixture.GetExampleListGenres();
            var exampleCategories = _fixture.GetExampleCategoryList();
            var random = new Random();
            exampleGenres.ForEach(genre =>
            {
                int relationsCount = random.Next(0, 3);
                for (int i = 0; i < relationsCount; i++)
                {
                    var selectedCategoryIndex = random.Next(0, exampleCategories.Count - 1);
                    var selected = exampleCategories[selectedCategoryIndex];
                    if (!genre.Categories.Contains(selected.Id))
                        genre.AddCategory(selected.Id);
                }
            });
            var genresCategories = new List<GenresCategories>();
            exampleGenres.ForEach(genre => genre.Categories.ToList().ForEach(
                category => genresCategories.Add(new GenresCategories(category, genre.Id)))
                );
            await dbContext.AddRangeAsync(exampleGenres);
            await dbContext.AddRangeAsync(exampleCategories);
            await dbContext.AddRangeAsync(genresCategories);
            await dbContext.SaveChangesAsync();
            var actDbContext = _fixture.CreateDbContext(true);

            var useCase = new UseCase.ListGenres(new GenreRepository(actDbContext), new CategoryRespository(actDbContext));
            var input = new UseCase.ListGenresInput(1, 20);
            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().HaveCount(exampleGenres.Count);
            output.Total.Should().Be(exampleGenres.Count);
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Items.ToList().ForEach(outputItem =>
            {
                var exampleItem = exampleGenres
                    .Find(example => example.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.IsActive.Should().Be(exampleItem!.IsActive);

                var outputItemCategoryIds = outputItem.Categories.Select(x => x.Id).ToList();
                outputItemCategoryIds.Should().BeEquivalentTo(exampleItem.Categories);
                outputItem.Categories.ToList().ForEach(outputCategory =>
                {
                    var exampleCategory = exampleCategories.Find(x => x.Id == outputCategory.Id);
                    exampleCategory.Should().NotBeNull();
                    outputCategory.Name.Should().Be(exampleCategory!.Name);
                });
            });
        }

        [Theory(DisplayName = (nameof(ListGenresPaginated)))]
        [Trait("Integration/Application", "ListGenres - Use Cases")]
        [InlineData(10, 1, 5, 5)]
        [InlineData(10, 2, 5, 5)]
        [InlineData(7, 2, 5, 2)]
        [InlineData(7, 3, 5, 0)]
        public async Task ListGenresPaginated(
            int quantityToGenerate,
            int page,
            int perPage,
            int expectedQuantityItems
            )
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleGenres = _fixture.GetExampleListGenres(quantityToGenerate);
            var exampleCategories = _fixture.GetExampleCategoryList();
            var random = new Random();
            exampleGenres.ForEach(genre =>
            {
                int relationsCount = random.Next(0, 3);
                for (int i = 0; i < relationsCount; i++)
                {
                    var selectedCategoryIndex = random.Next(0, exampleCategories.Count - 1);
                    var selected = exampleCategories[selectedCategoryIndex];
                    if (!genre.Categories.Contains(selected.Id))
                        genre.AddCategory(selected.Id);
                }
            });
            var genresCategories = new List<GenresCategories>();
            exampleGenres.ForEach(genre => genre.Categories.ToList().ForEach(
                categoryId => genresCategories.Add(new GenresCategories(categoryId, genre.Id)))
                );
            await dbContext.AddRangeAsync(exampleGenres);
            await dbContext.AddRangeAsync(exampleCategories);
            await dbContext.AddRangeAsync(genresCategories);
            await dbContext.SaveChangesAsync();
            var actDbContext = _fixture.CreateDbContext(true);

            var useCase = new UseCase.ListGenres(new GenreRepository(actDbContext), new CategoryRespository(actDbContext));
            var input = new UseCase.ListGenresInput(page, perPage);
            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(exampleGenres.Count);
            output.Items.Should().HaveCount(expectedQuantityItems);
            output.Items.ToList().ForEach(outputItem =>
            {
                var exampleItem = exampleGenres
                    .Find(example => example.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.IsActive.Should().Be(exampleItem!.IsActive);

                var outputItemCategoryIds = outputItem.Categories.Select(x => x.Id).ToList();
                outputItemCategoryIds.Should().BeEquivalentTo(exampleItem.Categories);
                outputItem.Categories.ToList().ForEach(outputCategory =>
                {
                    var exampleCategory = exampleCategories.Find(x => x.Id == outputCategory.Id);
                    exampleCategory.Should().NotBeNull();
                    outputCategory.Name.Should().Be(exampleCategory!.Name);
                });
            });
        }

        [Theory(DisplayName = (nameof(SearchByText)))]
        [Trait("Integration/Application", "ListGenres - Use Cases")]
        [InlineData("Action", 1, 5, 1, 1)]
        [InlineData("Horror", 1, 5, 3, 3)]
        [InlineData("Horror", 2, 5, 0, 3)]
        [InlineData("Sci-fi", 1, 5, 4, 4)]
        [InlineData("Sci-fi", 1, 2, 2, 4)]
        [InlineData("Sci-fi", 2, 3, 1, 4)]
        [InlineData("Sci-fi Other", 1, 3, 0, 0)]
        [InlineData("Robots", 1, 5, 2, 2)]
        public async Task SearchByText(
            string search,
            int page,
            int perPage,
            int expectedQuantityItemsReturned,
            int expectedQuantityTotalItems
            )
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleGenres = _fixture
                .GetExampleGenresListByNames(new List<string>() {
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

            var exampleCategories = _fixture.GetExampleCategoryList();
            var random = new Random();
            exampleGenres.ForEach(genre =>
            {
                int relationsCount = random.Next(0, 3);
                for (int i = 0; i < relationsCount; i++)
                {
                    var selectedCategoryIndex = random.Next(0, exampleCategories.Count - 1);
                    var selected = exampleCategories[selectedCategoryIndex];
                    if (!genre.Categories.Contains(selected.Id))
                        genre.AddCategory(selected.Id);
                }
            });
            var genresCategories = new List<GenresCategories>();
            exampleGenres.ForEach(genre => genre.Categories.ToList().ForEach(
                categoryId => genresCategories.Add(new GenresCategories(categoryId, genre.Id)))
                );
            await dbContext.AddRangeAsync(exampleGenres);
            await dbContext.AddRangeAsync(exampleCategories);
            await dbContext.AddRangeAsync(genresCategories);
            await dbContext.SaveChangesAsync();
            var actDbContext = _fixture.CreateDbContext(true);

            var useCase = new UseCase.ListGenres(new GenreRepository(actDbContext), new CategoryRespository(actDbContext));
            var input = new UseCase.ListGenresInput(page, perPage, search);
            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(expectedQuantityTotalItems);
            output.Items.Should().HaveCount(expectedQuantityItemsReturned);
            output.Items.ToList().ForEach(outputItem =>
            {
                var exampleItem = exampleGenres
                    .Find(example => example.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Contain(search);
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.IsActive.Should().Be(exampleItem!.IsActive);

                var outputItemCategoryIds = outputItem.Categories.Select(x => x.Id).ToList();
                outputItemCategoryIds.Should().BeEquivalentTo(exampleItem.Categories);
                outputItem.Categories.ToList().ForEach(outputCategory =>
                {
                    var exampleCategory = exampleCategories.Find(x => x.Id == outputCategory.Id);
                    exampleCategory.Should().NotBeNull();
                    outputCategory.Name.Should().Be(exampleCategory!.Name);
                });
            });
        }

        [Theory(DisplayName = (nameof(SearchOrdered)))]
        [Trait("Integration/Application", "ListGenres - Use Cases")]
        [InlineData("name", "asc")]
        [InlineData("name", "desc")]
        [InlineData("id", "asc")]
        [InlineData("id", "desc")]
        [InlineData("createdAt", "asc")]
        [InlineData("createdAt", "desc")]
        [InlineData("", "asc")]
        public async Task SearchOrdered(string orderBy, string order)
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleGenres = _fixture.GetExampleListGenres();
            var exampleCategories = _fixture.GetExampleCategoryList();
            var random = new Random();
            exampleGenres.ForEach(genre =>
            {
                int relationsCount = random.Next(0, 3);
                for (int i = 0; i < relationsCount; i++)
                {
                    var selectedCategoryIndex = random.Next(0, exampleCategories.Count - 1);
                    var selected = exampleCategories[selectedCategoryIndex];
                    if (!genre.Categories.Contains(selected.Id))
                        genre.AddCategory(selected.Id);
                }
            });
            var genresCategories = new List<GenresCategories>();
            exampleGenres.ForEach(genre => genre.Categories.ToList().ForEach(
                categoryId => genresCategories.Add(new GenresCategories(categoryId, genre.Id)))
                );
            await dbContext.AddRangeAsync(exampleGenres);
            await dbContext.AddRangeAsync(exampleCategories);
            await dbContext.AddRangeAsync(genresCategories);
            await dbContext.SaveChangesAsync();
            var actDbContext = _fixture.CreateDbContext(true);

            var useCase = new UseCase.ListGenres(new GenreRepository(actDbContext), new CategoryRespository(actDbContext));
            var orderEnum = order == "asc" ? SearchOrder.Asc: SearchOrder.Desc;
            var input = new UseCase.ListGenresInput(1, 20, "", sort: orderBy, dir: orderEnum);
            var output = await useCase.Handle(input, CancellationToken.None);

            var expectedOrderedList = _fixture.CloneGenreListOrdered(exampleGenres, orderBy.ToLower(), orderEnum);

            output.Should().NotBeNull();
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(exampleGenres.Count);
            output.Items.Should().HaveCount(exampleGenres.Count);

            for (int i = 0; i < expectedOrderedList.Count; i++)
            {
                var expectedItem = expectedOrderedList[i];
                var outputItem = output.Items[i];
                expectedItem.Should().NotBeNull();
                outputItem.Name.Should().Be(expectedItem!.Name);
                outputItem.IsActive.Should().Be(expectedItem!.IsActive);

                var outputItemCategoryIds = outputItem.Categories.Select(x => x.Id).ToList();
                outputItemCategoryIds.Should().BeEquivalentTo(expectedItem.Categories);
                outputItem.Categories.ToList().ForEach(outputCategory =>
                {
                    var exampleCategory = exampleCategories.Find(x => x.Id == outputCategory.Id);
                    exampleCategory.Should().NotBeNull();
                    outputCategory.Name.Should().Be(exampleCategory!.Name);
                });
            }
        }
    }
}
