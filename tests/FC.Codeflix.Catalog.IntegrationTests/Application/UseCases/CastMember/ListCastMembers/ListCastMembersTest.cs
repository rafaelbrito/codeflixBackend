using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.Common;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.ListCastMembers
{
    [Collection(nameof(CastMemberUseCaseBaseFixture))]
    public class ListCastMembersTest
    {
        private readonly CastMemberUseCaseBaseFixture _fixture;
        public ListCastMembersTest(CastMemberUseCaseBaseFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = (nameof(SearchReturnsListAndTotal)))]
        [Trait("Integration/Application", "ListCastMembers - Use Cases")]
        public async Task SearchReturnsListAndTotal()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleCastMemberList = _fixture.GetExampleCastMemberList(10);
            await dbContext.AddRangeAsync(exampleCastMemberList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var repository = new CastMemberRepository(dbContext);
            var searchInput = new UseCase.ListCastMembersInput(1, 20, "", "", SearchOrder.Asc);
            var useCase = new UseCase.ListCastMembers(repository);

            var output = await useCase.Handle(searchInput, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.Page.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(exampleCastMemberList.Count);
            output.Items.Should().HaveCount(exampleCastMemberList.Count);
            foreach (CastMemberModelOutput outputItem in output.Items)
            {
                var exampleItem = exampleCastMemberList.Find(
                    castMembers => castMembers.Id == outputItem.Id
                    );
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.Type.Should().Be(exampleItem!.Type);
            }
        }

        [Fact(DisplayName = (nameof(ListEmpty)))]
        [Trait("Integration/Application", "ListCastMembers - Use Cases")]
        public async Task ListEmpty()
        {
            var categoryRespository = new CastMemberRepository(_fixture.CreateDbContext());
            var searchInput = new UseCase.ListCastMembersInput(1, 20, "", "", SearchOrder.Asc);
            var useCase = new UseCase.ListCastMembers(categoryRespository);

            var output = await useCase.Handle(searchInput, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.Page.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
        }

        [Theory(DisplayName = (nameof(SearchReturnsPagineted)))]
        [Trait("Integration/Application", "ListCastMembers - Use Cases")]
        [InlineData(10, 1, 5, 5)]
        [InlineData(10, 2, 5, 5)]
        [InlineData(7, 2, 5, 2)]
        [InlineData(7, 3, 5, 0)]
        public async Task SearchReturnsPagineted(
        int quantityToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems
        )
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleCastMemberList = _fixture.GetExampleCastMemberList(quantityToGenerate);
            await dbContext.AddRangeAsync(exampleCastMemberList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var repository = new CastMemberRepository(dbContext);
            var searchInput = new UseCase.ListCastMembersInput(page, perPage, "", "", SearchOrder.Asc);
            var useCase = new UseCase.ListCastMembers(repository);

            var output = await useCase.Handle(searchInput, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.Page.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(exampleCastMemberList.Count);
            output.Items.Should().HaveCount(expectedQuantityItems);
            foreach (CastMemberModelOutput outputItem in output.Items)
            {
                var exampleItem = exampleCastMemberList.Find(
                    castMembers => castMembers.Id == outputItem.Id
                    );
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.Type.Should().Be(exampleItem!.Type);
            }
        }

        [Theory(DisplayName = (nameof(SearchByText)))]
        [Trait("Integration/Application", "ListCastMembers - Use Cases")]
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
            var exampleNameList = new List<string>
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
            var exampleCastMemberList = _fixture.GetExampleCastMemberListWithNames(exampleNameList);
            await dbContext.AddRangeAsync(exampleCastMemberList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var repository = new CastMemberRepository(dbContext);
            var searchInput = new UseCase.ListCastMembersInput(page, perPage, search, "", SearchOrder.Asc);
            var useCase = new UseCase.ListCastMembers(repository);

            var output = await useCase.Handle(searchInput, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.Page.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(expectedQuantityTotalItems);
            output.Items.Should().HaveCount(expectedQuantityItemsReturned);
            foreach (CastMemberModelOutput outputItem in output.Items)
            {
                var exampleItem = exampleCastMemberList.Find(
                    castMembers => castMembers.Id == outputItem.Id
                    );
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.Type.Should().Be(exampleItem!.Type);
            }
        }

        [Theory(DisplayName = (nameof(SearchOrdered)))]
        [Trait("Integration/Application", "ListCastMembers - Use Cases")]
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
            var exampleCastMemberList = _fixture.GetExampleCastMemberList();
            await dbContext.AddRangeAsync(exampleCastMemberList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var respository = new CastMemberRepository(dbContext);
            var useCaseOrder = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
            var searchInput = new UseCase.ListCastMembersInput(1, 10, "", orderBy, useCaseOrder);
            var useCase = new UseCase.ListCastMembers(respository);

            var output = await useCase.Handle(searchInput, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.Page.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(exampleCastMemberList.Count);
            output.Items.Should().HaveCount(exampleCastMemberList.Count);

            var expectedOrderedList = _fixture.CloneCastMembersListOrdered(exampleCastMemberList, orderBy, useCaseOrder);
            for (int i = 0; i < expectedOrderedList.Count; i++)
            {
                var expectedItem = expectedOrderedList[i];
                var outputItem = output.Items[i];
                outputItem.Should().NotBeNull();
                outputItem.Name.Should().Be(expectedItem!.Name);
                outputItem.Type.Should().Be(expectedItem!.Type);
            }
        }
    }
}
