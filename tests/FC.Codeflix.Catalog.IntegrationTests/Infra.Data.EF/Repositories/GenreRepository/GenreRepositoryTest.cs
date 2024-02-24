using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Infra.Data.EF.Model;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.GenreRepository
{
    [Collection(nameof(GenreRepositoryTestFixture))]
    public class GenreRepositoryTest
    {
        private readonly GenreRepositoryTestFixture _fixture;

        public GenreRepositoryTest(GenreRepositoryTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = (nameof(Insert)))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        public async Task Insert()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleGenre = _fixture.GetExampleGenre();
            var categoryListExample = _fixture.GetExampleCategoryList(3);
            categoryListExample.ForEach(
                category => exampleGenre.AddCategory(category.Id)
                );
            await dbContext.Categories.AddRangeAsync(categoryListExample);
            var genreRespository = new Repository.GenreRepository(dbContext);
            await dbContext.SaveChangesAsync(CancellationToken.None);


            await genreRespository.Insert(exampleGenre, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var assertsDbContext = _fixture.CreateDbContext(true);
            var dbGenre = await assertsDbContext
                .Genres.FindAsync(exampleGenre.Id);
            dbGenre.Should().NotBeNull();
            dbGenre!.Name.Should().Be(exampleGenre.Name);
            dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
            dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            var genreCategoriesRelations = await assertsDbContext
                .GenresCategories.Where(r => r.GenreId == exampleGenre.Id)
                .ToListAsync();
            genreCategoriesRelations.Should()
                .HaveCount(categoryListExample.Count);
            genreCategoriesRelations.ForEach(relation =>
            {
                var expectedCategory = categoryListExample
                    .FirstOrDefault(x => x.Id == relation.CategoryId);
                expectedCategory.Should().NotBeNull();
            });
        }

        [Fact(DisplayName = (nameof(Get)))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        public async Task Get()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleGenre = _fixture.GetExampleGenre();
            var categoryListExample = _fixture.GetExampleCategoryList(3);
            categoryListExample.ForEach(
                category => exampleGenre.AddCategory(category.Id)
                );
            await dbContext.Categories.AddRangeAsync(categoryListExample);
            await dbContext.Genres.AddAsync(exampleGenre);
            foreach (var categoryId in exampleGenre.Categories)
            {
                var relation = new GenresCategories(categoryId, exampleGenre.Id);
                await dbContext.AddAsync(relation);
            }
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var genreRespository = new Repository.GenreRepository(_fixture.CreateDbContext(true));

            var genreFromRepository = await genreRespository.Get(exampleGenre.Id, CancellationToken.None);

            var assertsDbContext = _fixture.CreateDbContext(true);
            genreFromRepository.Should().NotBeNull();
            genreFromRepository!.Name.Should().Be(exampleGenre.Name);
            genreFromRepository.IsActive.Should().Be(exampleGenre.IsActive);
            genreFromRepository.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            genreFromRepository.Categories.Should()
                .HaveCount(categoryListExample.Count);
            foreach (var categoryId in genreFromRepository.Categories)
            {
                var expectedCategory = categoryListExample
                    .FirstOrDefault(x => x.Id == categoryId);
                expectedCategory.Should().NotBeNull();
            }
        }

        [Fact(DisplayName = (nameof(GetThrowWhenNotFound)))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        public async Task GetThrowWhenNotFound()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleNotFoundGuid = Guid.NewGuid();
            var exampleGenre = _fixture.GetExampleGenre();
            var categoryListExample = _fixture.GetExampleCategoryList(3);
            categoryListExample.ForEach(
                category => exampleGenre.AddCategory(category.Id)
                );
            await dbContext.Categories.AddRangeAsync(categoryListExample);
            await dbContext.Genres.AddAsync(exampleGenre);
            foreach (var categoryId in exampleGenre.Categories)
            {
                var relation = new GenresCategories(categoryId, exampleGenre.Id);
                await dbContext.AddAsync(relation);
            }
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var genreRespository = new Repository.GenreRepository(_fixture.CreateDbContext(true));

            var action = async () => await genreRespository
            .Get(exampleNotFoundGuid, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Genre '{exampleNotFoundGuid}' not found.");
        }

        [Fact(DisplayName = (nameof(Delete)))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        public async Task Delete()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleGenre = _fixture.GetExampleGenre();
            var categoryListExample = _fixture.GetExampleCategoryList(3);
            categoryListExample.ForEach(
                category => exampleGenre.AddCategory(category.Id)
                );
            await dbContext.Categories.AddRangeAsync(categoryListExample);
            await dbContext.Genres.AddAsync(exampleGenre);
            foreach (var categoryId in exampleGenre.Categories)
            {
                var relation = new GenresCategories(categoryId, exampleGenre.Id);
                await dbContext.AddAsync(relation);
            }
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var repositoryDbContext = _fixture.CreateDbContext(true);
            var genreRespository = new Repository.GenreRepository(repositoryDbContext);

            await genreRespository.Delete(exampleGenre, CancellationToken.None);
            await repositoryDbContext.SaveChangesAsync(CancellationToken.None);

            var assertsDbContext = _fixture.CreateDbContext(true);
            var dbGenre = await assertsDbContext.Genres.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == exampleGenre.Id);
            dbGenre.Should().BeNull();
            var categoriesIdsList = await assertsDbContext.GenresCategories.AsNoTracking()
                .Where(x => x.GenreId == exampleGenre.Id)
                .Select(x => x.CategoryId)
                .ToListAsync();
            categoriesIdsList.Should().HaveCount(0);
        }

        [Fact(DisplayName = (nameof(Update)))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        public async Task Update()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleGenre = _fixture.GetExampleGenre();
            var categoryListExample = _fixture.GetExampleCategoryList(3);
            categoryListExample.ForEach(
                category => exampleGenre.AddCategory(category.Id)
                );
            await dbContext.Categories.AddRangeAsync(categoryListExample);
            await dbContext.Genres.AddAsync(exampleGenre);
            foreach (var categoryId in exampleGenre.Categories)
            {
                var relation = new GenresCategories(categoryId, exampleGenre.Id);
                await dbContext.AddAsync(relation);
            }
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var genreRespository = new Repository.GenreRepository(_fixture.CreateDbContext(true));

            exampleGenre.Update(_fixture.GetValidGenreName());
            if (exampleGenre.IsActive)
                exampleGenre.Deactivate();
            else
                exampleGenre.Activate();

            await genreRespository.Update(exampleGenre, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var assertsDbContext = _fixture.CreateDbContext(true);
            var dbGenre = await assertsDbContext
                .Genres.FindAsync(exampleGenre.Id);
            dbGenre.Should().NotBeNull();
            dbGenre!.Name.Should().Be(exampleGenre.Name);
            dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
            dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            var genreCategoriesRelations = await assertsDbContext
                .GenresCategories.Where(r => r.GenreId == exampleGenre.Id)
                .ToListAsync();
            genreCategoriesRelations.Should()
                .HaveCount(categoryListExample.Count);
            genreCategoriesRelations.ForEach(relation =>
            {
                var expectedCategory = categoryListExample
                    .FirstOrDefault(x => x.Id == relation.CategoryId);
                expectedCategory.Should().NotBeNull();
            });
        }

        [Fact(DisplayName = (nameof(UpdateRemovingRelations)))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        public async Task UpdateRemovingRelations()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleGenre = _fixture.GetExampleGenre();
            var categoryListExample = _fixture.GetExampleCategoryList(3);
            categoryListExample.ForEach(
                category => exampleGenre.AddCategory(category.Id)
                );
            await dbContext.Categories.AddRangeAsync(categoryListExample);
            await dbContext.Genres.AddAsync(exampleGenre);
            foreach (var categoryId in exampleGenre.Categories)
            {
                var relation = new GenresCategories(categoryId, exampleGenre.Id);
                await dbContext.AddAsync(relation);
            }
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var genreRespository = new Repository.GenreRepository(dbContext);

            exampleGenre.Update(_fixture.GetValidGenreName());
            if (exampleGenre.IsActive)
                exampleGenre.Deactivate();
            else
                exampleGenre.Activate();
            exampleGenre.RemoveAllCategory();
            await genreRespository.Update(exampleGenre, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var dbGenre = await dbContext
                .Genres.FindAsync(exampleGenre.Id);
            dbGenre.Should().NotBeNull();
            dbGenre!.Name.Should().Be(exampleGenre.Name);
            dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
            dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            var genreCategoriesRelations = await dbContext
                .GenresCategories.Where(r => r.GenreId == exampleGenre.Id)
                .ToListAsync();
            genreCategoriesRelations.Should()
                .HaveCount(0);
        }

        [Fact(DisplayName = (nameof(UpdateReplacingRelations)))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        public async Task UpdateReplacingRelations()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleGenre = _fixture.GetExampleGenre();
            var categoryListExample = _fixture.GetExampleCategoryList(3);
            var UpdateCategoryListExample = _fixture.GetExampleCategoryList(4);
            categoryListExample.ForEach(
                category => exampleGenre.AddCategory(category.Id)
                );
            await dbContext.Categories.AddRangeAsync(categoryListExample);
            await dbContext.Categories.AddRangeAsync(UpdateCategoryListExample);
            await dbContext.Genres.AddAsync(exampleGenre);
            foreach (var categoryId in exampleGenre.Categories)
            {
                var relation = new GenresCategories(categoryId, exampleGenre.Id);
                await dbContext.AddAsync(relation);
            }
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var genreRepository = new Repository.GenreRepository(dbContext);

            exampleGenre.Update(_fixture.GetValidGenreName());
            if (exampleGenre.IsActive)
                exampleGenre.Deactivate();
            else
                exampleGenre.Activate();
            exampleGenre.RemoveAllCategory();
            UpdateCategoryListExample
                .ForEach(category => exampleGenre.AddCategory(category.Id));

            await genreRepository.Update(exampleGenre, CancellationToken.None);
            await dbContext.SaveChangesAsync();


            var dbGenre = await dbContext
                .Genres.FindAsync(exampleGenre.Id);
            dbGenre.Should().NotBeNull();
            dbGenre!.Name.Should().Be(exampleGenre.Name);
            dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
            dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            var genreCategoriesRelations = await dbContext
                .GenresCategories.Where(r => r.GenreId == exampleGenre.Id)
                .ToListAsync();
            genreCategoriesRelations.Should()
                .HaveCount(UpdateCategoryListExample.Count);
            genreCategoriesRelations.ForEach(relation =>
            {
                var expectedCategory = UpdateCategoryListExample
                    .FirstOrDefault(x => x.Id == relation.CategoryId);
                expectedCategory.Should().NotBeNull();
            });
        }

        [Fact(DisplayName = (nameof(SearchReturnsItemsAndTotal)))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        public async Task SearchReturnsItemsAndTotal()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleListGenres = _fixture.GetExampleListGenres();
            await dbContext.AddRangeAsync(exampleListGenres);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var genreRepository = new Repository.GenreRepository(dbContext);

            var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

            var searchResult = await genreRepository.Search(searchInput, CancellationToken.None);

            searchResult.Should().NotBeNull();
            searchResult.CurrentPage.Should().Be(searchInput.Page);
            searchResult.PerPage.Should().Be(searchInput.PerPage);
            searchResult.Total.Should().Be(exampleListGenres.Count);
            searchResult.Items.Should().NotBeNull();
            searchResult.Items.Should().HaveCount(exampleListGenres.Count);

            foreach (var resultItem in searchResult.Items)
            {
                var exampleGenre = exampleListGenres
                    .Find(x => x.Id == resultItem.Id);
                exampleGenre.Should().NotBeNull();
                resultItem!.Name.Should().Be(exampleGenre!.Name);
                resultItem.IsActive.Should().Be(exampleGenre.IsActive);
                resultItem.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            }
        }

        [Fact(DisplayName = (nameof(SearchReturnsRelations)))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        public async Task SearchReturnsRelations()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleListGenres = _fixture.GetExampleListGenres(10);
            await dbContext.AddRangeAsync(exampleListGenres);
            var random = new Random();
            exampleListGenres.ForEach(exampleGenre =>
            {
                var categoriesListToRelation =
                    _fixture.GetExampleCategoryList(random.Next(0, 4));
                if (categoriesListToRelation.Count > 0)
                {
                    categoriesListToRelation.ForEach(
                        category => exampleGenre.AddCategory(category.Id));
                    dbContext.Categories.AddRange(categoriesListToRelation);
                    var relationToAdd = categoriesListToRelation.Select(
                        category => new GenresCategories(category.Id, exampleGenre.Id)
                        ).ToList();
                    dbContext.GenresCategories.AddRange(relationToAdd);
                }
            });
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var genreRepository = new Repository.GenreRepository(dbContext);

            var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

            var searchResult = await genreRepository.Search(searchInput, CancellationToken.None);

            searchResult.Should().NotBeNull();
            searchResult.CurrentPage.Should().Be(searchInput.Page);
            searchResult.PerPage.Should().Be(searchInput.PerPage);
            searchResult.Total.Should().Be(exampleListGenres.Count);
            searchResult.Items.Should().NotBeNull();
            searchResult.Items.Should().HaveCount(exampleListGenres.Count);
            foreach (var resultItem in searchResult.Items)
            {
                var exampleGenre = exampleListGenres
                    .Find(x => x.Id == resultItem.Id);
                exampleGenre.Should().NotBeNull();
                resultItem!.Name.Should().Be(exampleGenre!.Name);
                resultItem.IsActive.Should().Be(exampleGenre.IsActive);
                resultItem.CreatedAt.Should().Be(exampleGenre.CreatedAt);
                resultItem.Categories.Should().HaveCount(exampleGenre.Categories.Count);
                resultItem.Categories.Should().BeEquivalentTo(exampleGenre.Categories);
            }
        }

        [Fact(DisplayName = (nameof(SearchReturnsEmptyWhenPersistenceIsEmpty)))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        public async Task SearchReturnsEmptyWhenPersistenceIsEmpty()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var genreRepository = new Repository.GenreRepository(dbContext);

            var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

            var searchResult = await genreRepository.Search(searchInput, CancellationToken.None);

            searchResult.Should().NotBeNull();
            searchResult.CurrentPage.Should().Be(searchInput.Page);
            searchResult.PerPage.Should().Be(searchInput.PerPage);
            searchResult.Total.Should().Be(0);
            searchResult.Items.Should().HaveCount(0);
        }

        [Theory(DisplayName = (nameof(SearchReturnsPaginated)))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
        [InlineData(10, 1, 5, 5)]
        [InlineData(10, 2, 5, 5)]
        [InlineData(7, 2, 5, 2)]
        [InlineData(7, 3, 5, 0)]
        public async Task SearchReturnsPaginated(
            int quantityToGenerate,
            int page,
            int perPage,
            int expectedQuantityItems
            )
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleListGenres = _fixture.GetExampleListGenres(quantityToGenerate);
            await dbContext.AddRangeAsync(exampleListGenres);
            var random = new Random();
            exampleListGenres.ForEach(exampleGenre =>
            {
                var categoriesListToRelation =
                    _fixture.GetExampleCategoryList(random.Next(0, 4));
                if (categoriesListToRelation.Count > 0)
                {
                    categoriesListToRelation.ForEach(
                        category => exampleGenre.AddCategory(category.Id));
                    dbContext.Categories.AddRange(categoriesListToRelation);
                    var relationToAdd = categoriesListToRelation.Select(
                        category => new GenresCategories(category.Id, exampleGenre.Id)
                        ).ToList();
                    dbContext.GenresCategories.AddRange(relationToAdd);
                }
            });
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var genreRepository = new Repository.GenreRepository(dbContext);

            var searchInput = new SearchInput(page, perPage, "", "", SearchOrder.Asc);

            var searchResult = await genreRepository.Search(searchInput, CancellationToken.None);

            searchResult.Should().NotBeNull();
            searchResult.CurrentPage.Should().Be(searchInput.Page);
            searchResult.PerPage.Should().Be(searchInput.PerPage);
            searchResult.Total.Should().Be(exampleListGenres.Count);
            searchResult.Items.Should().HaveCount(expectedQuantityItems);
            foreach (var resultItem in searchResult.Items)
            {
                var exampleGenre = exampleListGenres
                    .Find(x => x.Id == resultItem.Id);
                exampleGenre.Should().NotBeNull();
                resultItem!.Name.Should().Be(exampleGenre!.Name);
                resultItem.IsActive.Should().Be(exampleGenre.IsActive);
                resultItem.CreatedAt.Should().Be(exampleGenre.CreatedAt);
                resultItem.Categories.Should().HaveCount(exampleGenre.Categories.Count);
                resultItem.Categories.Should().BeEquivalentTo(exampleGenre.Categories);
            }
        }

        [Theory(DisplayName = (nameof(SearchByText)))]
        [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
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
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();

            var exampleGenresList = _fixture
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
            await dbContext.AddRangeAsync(exampleGenresList);
            var random = new Random();
            exampleGenresList.ForEach(exampleGenre =>
            {
                var categoriesListToRelation =
                    _fixture.GetExampleCategoryList(random.Next(0, 4));
                if (categoriesListToRelation.Count > 0)
                {
                    categoriesListToRelation.ForEach(
                        category => exampleGenre.AddCategory(category.Id));
                    dbContext.Categories.AddRange(categoriesListToRelation);
                    var relationToAdd = categoriesListToRelation.Select(
                        category => new GenresCategories(category.Id, exampleGenre.Id)
                        ).ToList();
                    dbContext.GenresCategories.AddRange(relationToAdd);
                }
            });
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var genreRepository = new Repository.GenreRepository(dbContext);

            var searchInput = new SearchInput(page, perPage, search, "", SearchOrder.Asc);

            var searchResult = await genreRepository.Search(searchInput, CancellationToken.None);

            searchResult.Should().NotBeNull();
            searchResult.CurrentPage.Should().Be(searchInput.Page);
            searchResult.PerPage.Should().Be(searchInput.PerPage);
            searchResult.Total.Should().Be(expectedQuantityTotalItems);
            searchResult.Items.Should().HaveCount(expectedQuantityItemsReturned);
            foreach (var resultItem in searchResult.Items)
            {
                var exampleGenre = exampleGenresList
                    .Find(x => x.Id == resultItem.Id);
                exampleGenre.Should().NotBeNull();
                resultItem!.Name.Should().Be(exampleGenre!.Name);
                resultItem.IsActive.Should().Be(exampleGenre.IsActive);
                resultItem.CreatedAt.Should().Be(exampleGenre.CreatedAt);
                resultItem.Categories.Should().HaveCount(exampleGenre.Categories.Count);
                resultItem.Categories.Should().BeEquivalentTo(exampleGenre.Categories);
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
        [InlineData("", "asc")]
        public async Task SearchOrdered(string orderBy, string order)
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleGenreList = _fixture.GetExampleListGenres(10);
            await dbContext.AddRangeAsync(exampleGenreList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var genreRepository = new Repository.GenreRepository(dbContext);
            var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
            var searchInput = new SearchInput(1, 20, "", orderBy.ToLower(), searchOrder);

            var output = await genreRepository.Search(searchInput, CancellationToken.None);

            var expectedOrderedList = _fixture.CloneGenreListOrdered(exampleGenreList, orderBy.ToLower(), searchOrder);


            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.CurrentPage.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(exampleGenreList.Count);
            output.Items.Should().HaveCount(exampleGenreList.Count);
            for (int i = 0; i < expectedOrderedList.Count; i++)
            {
                var expectedItem = expectedOrderedList[i];
                var outputItem = output.Items[i];
                expectedItem.Should().NotBeNull();
                outputItem.Should().NotBeNull();
                outputItem.Name.Should().Be(expectedItem.Name);
                outputItem.Id.Should().Be(expectedItem.Id);
                outputItem.IsActive.Should().Be(expectedItem.IsActive);
                outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
            }
        }
    }
}
