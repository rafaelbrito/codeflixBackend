using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using Moq;
using Xunit;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;

namespace FC.Codeflix.Catalog.UniTests.Application.CastMember.ListCastMember
{
    [Collection(nameof(ListCastMemberTestFixture))]
    public class ListCastMemberTest
    {
        private readonly ListCastMemberTestFixture _fixture;
        public ListCastMemberTest(ListCastMemberTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(List))]
        [Trait("Application", "ListCastMembers - Use Cases")]
        public async Task List()
        {
            var repositoryMock = new Mock<ICastMemberRepository>();
            var exampleCastMemberList = _fixture.GetExampleCastMemberList(3);
            var outputRepositorySearch = new SearchOutput<DomainEntity.CastMember>(
               currentPage: 1,
               perPage: 10,
               items: exampleCastMemberList,
               total: exampleCastMemberList.Count
               );

            repositoryMock.Setup(x => x.Search(
               It.IsAny<SearchInput>(),
               It.IsAny<CancellationToken>()
              )
           ).ReturnsAsync(outputRepositorySearch);

            var input = new ListCastMembersInput(1, 10, "", "", SearchOrder.Asc);

            var useCase = new UseCase.ListCastMembers(repositoryMock.Object);
            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(outputRepositorySearch.CurrentPage);
            output.PerPage.Should().Be(outputRepositorySearch.PerPage);
            output.Total.Should().Be(outputRepositorySearch.Total);
            output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
            ((List<CastMemberModelOutput>)output.Items).ForEach(outputItem =>
            {
                var repositoryGenre = outputRepositorySearch.Items
                .FirstOrDefault(x => x.Id == outputItem.Id);
                outputItem.Should().NotBeNull();
                repositoryGenre.Should().NotBeNull();
                outputItem.Name.Should().Be(repositoryGenre!.Name);
                outputItem.Type.Should().Be(repositoryGenre.Type);
                outputItem.CreatedAt.Should().Be(repositoryGenre.CreatedAt);
            }
            );
            repositoryMock.Verify(x => x.Search(
                It.Is<SearchInput>(
                    x => x.Page == input.Page
                    && x.PerPage == input.PerPage
                    && x.Search == input.Search
                    && x.Order == input.Dir
                    && x.OrderBy == input.Sort
                ),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = nameof(List))]
        [Trait("Application", "ListCastMembers - Use Cases")]
        public async Task ReturnsEmptyWhenEmpty()
        {
            var repositoryMock = new Mock<ICastMemberRepository>();
            var exampleCastMemberList = new List<DomainEntity.CastMember>();
            var outputRepositorySearch = new SearchOutput<DomainEntity.CastMember>(
               currentPage: 1,
               perPage: 10,
               items: exampleCastMemberList,
               total: exampleCastMemberList.Count
               );

            repositoryMock.Setup(x => x.Search(
               It.IsAny<SearchInput>(),
               It.IsAny<CancellationToken>()
              )
           ).ReturnsAsync(outputRepositorySearch);

            var input = new ListCastMembersInput(1, 10, "", "", SearchOrder.Asc);

            var useCase = new UseCase.ListCastMembers(repositoryMock.Object);
            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(outputRepositorySearch.CurrentPage);
            output.PerPage.Should().Be(outputRepositorySearch.PerPage);
            output.Total.Should().Be(outputRepositorySearch.Total);
            output.Items.Should().HaveCount(exampleCastMemberList.Count);
            repositoryMock.Verify(x => x.Search(
                It.Is<SearchInput>(
                    x => x.Page == input.Page
                    && x.PerPage == input.PerPage
                    && x.Search == input.Search
                    && x.Order == input.Dir
                    && x.OrderBy == input.Sort
                ),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
