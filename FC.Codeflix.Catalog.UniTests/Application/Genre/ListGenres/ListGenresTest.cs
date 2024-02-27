using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenres;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;
using Xunit;
using FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenres;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;

namespace FC.Codeflix.Catalog.UniTests.Application.Genre.ListGenres
{
    [Collection(nameof(ListGenresTestFixture))]
    public class ListGenresTest
    {
        private readonly ListGenresTestFixture _fixture;

        public ListGenresTest(ListGenresTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(ListGenres))]
        [Trait("Application", "ListGenres - Use Cases")]
        public async Task ListGenres()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
            var genreListExample = _fixture.GetExampleGenreList();

            var input = _fixture.GetExampleInput();
            var outputRepositorySearch = new SearchOutput<DomainEntity.Genre>(
                currentPage: input.Page,
                perPage: input.PerPage,
                items: (IReadOnlyList<DomainEntity.Genre>)genreListExample,
                total: new Random().Next(50, 200)
                );

            genreRepositoryMock.Setup(x => x.Search(
                It.IsAny<SearchInput>(),
                It.IsAny<CancellationToken>()
               )
            ).ReturnsAsync(outputRepositorySearch);

            var useCase = new UseCase.ListGenres(
                genreRepositoryMock.Object,
                categoryRepositoryMock.Object);

            ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(outputRepositorySearch.CurrentPage);
            output.PerPage.Should().Be(outputRepositorySearch.PerPage);
            output.Total.Should().Be(outputRepositorySearch.Total);
            output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
            ((List<GenreModelOutput>)output.Items).ForEach(outputItem =>
            {
                var repositoryGenre = outputRepositorySearch.Items
                .FirstOrDefault(x => x.Id == outputItem.Id);
                outputItem.Should().NotBeNull();
                repositoryGenre.Should().NotBeNull();
                outputItem.Name.Should().Be(repositoryGenre!.Name);
                outputItem.IsActive.Should().Be(repositoryGenre.IsActive);
                outputItem.CreatedAt.Should().Be(repositoryGenre.CreatedAt);
                outputItem.Categories.Should().HaveCount(repositoryGenre.Categories.Count);
                foreach (var expectedid in repositoryGenre.Categories)
                    outputItem.Categories.Should().Contain(relation => relation.Id == expectedid);
            });

            genreRepositoryMock.Verify(x => x.Search(
                It.Is<SearchInput>(
                    searchInput => searchInput.Page == input.Page
                    && searchInput.PerPage == input.PerPage
                    && searchInput.Search == input.Search
                    && searchInput.OrderBy == input.Sort
                    && searchInput.Order == input.Dir
                    ),
                It.IsAny<CancellationToken>()), Times.Once);

            var expectedIds = genreListExample
                       .SelectMany(genre => genre.Categories)
                       .Distinct().ToList();

            categoryRepositoryMock.Verify(x => x.GetListByIds(
                It.Is<List<Guid>>(parameterList => expectedIds.All(
                    id => parameterList.Contains(id)
                    && parameterList.Count == expectedIds.Count)
                    ), 
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = nameof(ListEmpty))]
        [Trait("Application", "ListGenres - Use Cases")]
        public async Task ListEmpty()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();

            var input = _fixture.GetExampleInput();
            var outputRepositorySearch = new SearchOutput<DomainEntity.Genre>(
                currentPage: input.Page,
                perPage: input.PerPage,
                items: (IReadOnlyList<DomainEntity.Genre>)new List<DomainEntity.Genre>(),
                total: new Random().Next(50, 200)
                );

            genreRepositoryMock.Setup(x => x.Search(
                It.IsAny<SearchInput>(),
                It.IsAny<CancellationToken>()
               )
            ).ReturnsAsync(outputRepositorySearch);

            var useCase = new UseCase.ListGenres(
                genreRepositoryMock.Object,
                categoryRepositoryMock.Object);

            ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(outputRepositorySearch.CurrentPage);
            output.PerPage.Should().Be(outputRepositorySearch.PerPage);
            output.Total.Should().Be(outputRepositorySearch.Total);
            output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);

            genreRepositoryMock.Verify(x => x.Search(
                It.Is<SearchInput>(
                    searchInput => searchInput.Page == input.Page
                    && searchInput.PerPage == input.PerPage
                    && searchInput.Search == input.Search
                    && searchInput.OrderBy == input.Sort
                    && searchInput.Order == input.Dir
                    ),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = nameof(ListUsingDefaultValues))]
        [Trait("Application", "ListGenres - Use Cases")]
        public async Task ListUsingDefaultValues()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
            var outputRepositorySearch = new SearchOutput<DomainEntity.Genre>(
                currentPage: 1,
                perPage: 15,
                items: (IReadOnlyList<DomainEntity.Genre>)new List<DomainEntity.Genre>(),
                total: 0
                );

            genreRepositoryMock.Setup(x => x.Search(
                It.IsAny<SearchInput>(),
                It.IsAny<CancellationToken>()
               )
            ).ReturnsAsync(outputRepositorySearch);

            var useCase = new UseCase.ListGenres(
                genreRepositoryMock.Object, 
                categoryRepositoryMock.Object);

            ListGenresOutput output = await useCase.Handle(new UseCase.ListGenresInput(), CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(outputRepositorySearch.CurrentPage);
            output.PerPage.Should().Be(outputRepositorySearch.PerPage);
            output.Total.Should().Be(outputRepositorySearch.Total);
            output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);

            genreRepositoryMock.Verify(x => x.Search(
                It.Is<SearchInput>(
                    searchInput => searchInput.Page == 1
                    && searchInput.PerPage == 15
                    && searchInput.Search == ""
                    && searchInput.OrderBy == ""
                    && searchInput.Order == SearchOrder.Asc
                    ),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
