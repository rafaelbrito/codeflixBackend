using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using Xunit;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CastMemberRepository
{
    [Collection(nameof(CastMemberRepositoryTestFixture))]
    public class CastMemberRepositoryTest
    {
        private readonly CastMemberRepositoryTestFixture _fixture;

        public CastMemberRepositoryTest(CastMemberRepositoryTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = (nameof(Insert)))]
        [Trait("Integration/Ifra.Data", "CastMemberRepository - Repositories")]
        public async Task Insert()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleCastMember = _fixture.GetExampleCastMember();
            var repository = new Repository.CastMemberRepository(dbContext);

            await repository.Insert(exampleCastMember, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var assertsDbContext = _fixture.CreateDbContext(true);
            var castMember = await assertsDbContext.CastMembers.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == exampleCastMember.Id);
            castMember.Should().NotBeNull();
            castMember!.Name.Should().Be(exampleCastMember.Name);
            castMember.Type.Should().Be(exampleCastMember.Type);
        }

        [Fact(DisplayName = (nameof(Get)))]
        [Trait("Integration/Ifra.Data", "CastMemberRepository - Repositories")]
        public async Task Get()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleCastMember = _fixture.GetExampleCastMemberList();
            var targetCastMember = exampleCastMember[5];
            var repository = new Repository.CastMemberRepository(dbContext);
            await dbContext.CastMembers.AddRangeAsync(exampleCastMember, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var castMember = await repository.Get(targetCastMember.Id, CancellationToken.None);
            castMember.Should().NotBeNull();
            castMember!.Name.Should().Be(targetCastMember.Name);
            castMember.Type.Should().Be(targetCastMember.Type);
        }

        [Fact(DisplayName = (nameof(GetThrowWhenNotFound)))]
        [Trait("Integration/Ifra.Data", "CastMemberRepository - Repositories")]
        public async Task GetThrowWhenNotFound()
        {
            var randomGuid = Guid.NewGuid();
            var repository = new Repository.CastMemberRepository(_fixture.CreateDbContext());

            var action = async () => await repository.Get(randomGuid, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"CastMember '{randomGuid}' not found.");
        }

        [Fact(DisplayName = (nameof(Update)))]
        [Trait("Integration/Ifra.Data", "CastMemberRepository - Repositories")]
        public async Task Update()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleCastMember = _fixture.GetExampleCastMemberList();
            var targetCastMember = exampleCastMember[5];
            var newName = _fixture.GetValidName();
            var newType = _fixture.GetRandomCastMemberType();
            var repository = new Repository.CastMemberRepository(dbContext);
            await dbContext.CastMembers.AddRangeAsync(exampleCastMember, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            targetCastMember.Update(newName, newType);
            await repository.Update(targetCastMember, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var castMember = dbContext.CastMembers
                .FirstOrDefault(x => x.Id == targetCastMember.Id);
            castMember.Should().NotBeNull();
            castMember!.Name.Should().Be(newName);
            castMember.Type.Should().Be(newType);
        }

        [Fact(DisplayName = (nameof(Delete)))]
        [Trait("Integration/Ifra.Data", "CastMemberRepository - Repositories")]
        public async Task Delete()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleCastMember = _fixture.GetExampleCastMemberList();
            var targetCastMember = exampleCastMember[5];
            var repository = new Repository.CastMemberRepository(dbContext);
            await dbContext.CastMembers.AddRangeAsync(exampleCastMember, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            await repository.Delete(targetCastMember, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var assertsDbContext = _fixture.CreateDbContext(true);
            var castMember = assertsDbContext.CastMembers.AsNoTracking().ToList();
            castMember.Should().NotBeNull();
            castMember.Should().HaveCount(9);
            castMember.Should().NotContain(targetCastMember);
        }

        [Fact(DisplayName = (nameof(Search)))]
        [Trait("Integration/Ifra.Data", "CastMemberRepository - Repositories")]
        public async Task Search()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleCastMember = _fixture.GetExampleCastMemberList();
            await dbContext.AddRangeAsync(exampleCastMember);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var repository = new Repository.CastMemberRepository(_fixture.CreateDbContext(true));

            var search = await repository.Search(
                new SearchInput(1, 20, "", "", SearchOrder.Asc), CancellationToken.None);
            search.Should().NotBeNull();
            search.CurrentPage.Should().Be(1);
            search.PerPage.Should().Be(20);
            search.Items.Should().HaveCount(10);
            search.Items.ToList().ForEach(
                resultItem =>
                {
                    var exampleItem = exampleCastMember.Find(x => x.Id == resultItem.Id);
                    exampleItem.Should().NotBeNull();
                    resultItem.Name.Should().Be(exampleItem!.Name);
                    resultItem.Type.Should().Be(exampleItem.Type);
                });
        }

        [Fact(DisplayName = (nameof(SearchReturnsEmptyWhenEmpty)))]
        [Trait("Integration/Ifra.Data", "CastMemberRepository - Repositories")]
        public async Task SearchReturnsEmptyWhenEmpty()
        {
            var dbContext = _fixture.CreateDbContext();
            var repository = new Repository.CastMemberRepository(_fixture.CreateDbContext(true));
            var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

            var search = await repository.Search(searchInput, CancellationToken.None);

            search.Should().NotBeNull();
            search.Items.Should().NotBeNull();
            search.CurrentPage.Should().Be(searchInput.Page);
            search.PerPage.Should().Be(searchInput.PerPage);
            search.Total.Should().Be(0);
            search.Items.Should().HaveCount(0);
        }

        [Theory(DisplayName = (nameof(SearchReturnsPagineted)))]
        [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
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
            var dbContext = _fixture.CreateDbContext();
            var exampleCastMember = _fixture.GetExampleCastMemberList(quantityToGenerate);
            await dbContext.AddRangeAsync(exampleCastMember);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var repository = new Repository.CastMemberRepository(_fixture.CreateDbContext(true));
            var searchInput = new SearchInput(page, perPage, "", "", SearchOrder.Asc);


            var search = await repository.Search(searchInput, CancellationToken.None);
            search.Should().NotBeNull();
            search.CurrentPage.Should().Be(searchInput.Page);
            search.PerPage.Should().Be(searchInput.PerPage);
            search.Items.Should().HaveCount(expectedQuantityItems);
            search.Total.Should().Be(quantityToGenerate);
            search.Items.ToList().ForEach(
                resultItem =>
                {
                    var exampleItem = exampleCastMember.Find(x => x.Id == resultItem.Id);
                    exampleItem.Should().NotBeNull();
                    resultItem.Name.Should().Be(exampleItem!.Name);
                    resultItem.Type.Should().Be(exampleItem.Type);
                });
        }

        [Theory(DisplayName = (nameof(SearchByText)))]
        [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
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
            var exampleCastMember = _fixture
                .GetExampleCastMemberListWithNames(new List<string>() {
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
            await dbContext.AddRangeAsync(exampleCastMember);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var repository = new Repository.CastMemberRepository(_fixture.CreateDbContext(true));

            var searchInput = new SearchInput(page, perPage, search, "", SearchOrder.Asc);
            var output = await repository.Search(searchInput, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.CurrentPage.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(expectedQuantityTotalItems);
            output.Items.Should().HaveCount(expectedQuantityItemsReturned);
            output.Items.ToList().ForEach(
                resultItem =>
                {
                    var exampleItem = exampleCastMember.Find(x => x.Id == resultItem.Id);
                    exampleItem.Should().NotBeNull();
                    resultItem.Name.Should().Be(exampleItem!.Name);
                    resultItem.Type.Should().Be(exampleItem.Type);
                });
        }

        [Theory(DisplayName = (nameof(SearchOrdered)))]
        [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
        [InlineData("name", "asc")]
        [InlineData("name", "desc")]
        [InlineData("id", "asc")]
        [InlineData("id", "desc")]
        [InlineData("type", "asc")]
        [InlineData("type", "desc")]
        [InlineData("createdAt", "asc")]
        [InlineData("createdAt", "desc")]
        [InlineData("", "desc")]
        public async Task SearchOrdered(string orderBy, string order)
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleCastMemberList = _fixture.GetExampleCastMemberList(10);
            await dbContext.AddRangeAsync(exampleCastMemberList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var repository = new Repository.CastMemberRepository(dbContext);
            var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
            var searchInput = new SearchInput(1, 20, "", orderBy.ToLower(), searchOrder);

            var output = await repository.Search(searchInput, CancellationToken.None);

            var expectedOrderedList = _fixture.CloneCastMembersListOrdered(exampleCastMemberList, orderBy.ToLower(), searchOrder);


            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.CurrentPage.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(exampleCastMemberList.Count);
            output.Items.Should().HaveCount(exampleCastMemberList.Count);
            for (int i = 0; i < expectedOrderedList.Count; i++)
            {
                var expectedItem = expectedOrderedList[i];
                var outputItem = output.Items[i];
                expectedItem.Should().NotBeNull();
                outputItem.Should().NotBeNull();
                outputItem.Id.Should().Be(expectedItem.Id);
                outputItem.Name.Should().Be(expectedItem.Name);
                outputItem.Type.Should().Be(expectedItem.Type);
                outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
            }
        }
    }
}
