using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.ListVideos;
using FC.Codeflix.Catalog.Domain.Repository;
using Moq;
using Xunit;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Application.UseCases.Video.ListVideos;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Extensions;

namespace FC.Codeflix.Catalog.UniTests.Application.Video.ListVideos
{
    [Collection(nameof(ListVideosTestFixture))]
    public class ListVideosTest
    {
        private readonly ListVideosTestFixture _fixture;
        private readonly Mock<IVideoRepository> _videoRepositoryMock;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<IGenreRepository> _genreRepositoryMock;
        private readonly Mock<ICastMemberRepository> _castMemberRepositoryMock;
        private readonly UseCase.ListVideos _useCases;
        public ListVideosTest(ListVideosTestFixture fixture)
        {
            _fixture = fixture;
            _videoRepositoryMock = new Mock<IVideoRepository>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _genreRepositoryMock = new Mock<IGenreRepository>();
            _castMemberRepositoryMock = new Mock<ICastMemberRepository>();
            _useCases = new UseCase.ListVideos(
                _videoRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _genreRepositoryMock.Object
                );
        }

        [Fact(DisplayName = nameof(ListVideos))]
        [Trait("Application", "ListVideos - Use Cases")]
        public async Task ListVideos()
        {
            var exampleVideoList = _fixture.GetExampleVideoList();
            var outputRepositorySearch = new SearchOutput<DomainEntity.Video>(
               currentPage: 1,
               perPage: 10,
               items: exampleVideoList,
               total: exampleVideoList.Count
               );
            var input = new ListVideosInput(
                1,
                10,
                "",
                "",
                SearchOrder.Asc
                );

            _videoRepositoryMock.Setup(x => x.Search(
                It.Is<SearchInput>(x =>
                    x.Page == input.Page &&
                    x.PerPage == input.PerPage &&
                    x.Search == input.Search &&
                    x.OrderBy == input.Sort &&
                    x.Order == input.Dir),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(outputRepositorySearch);

            var output = await _useCases.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(exampleVideoList.Count);
            output.Items.Should().HaveCount(exampleVideoList.Count);
            output.Items.ToList().ForEach(outputItem =>
            {
                var videoExample = exampleVideoList.Find(x => x.Id == outputItem.Id);

                videoExample.Should().NotBeNull();
                outputItem.Should().NotBeNull();
                outputItem.Id.Should().Be(videoExample!.Id);
                outputItem.Title.Should().Be(videoExample.Title);
                outputItem.Description.Should().Be(videoExample.Description);
                outputItem.YearLauched.Should().Be(videoExample.YearLauched);
                outputItem.Opened.Should().Be(videoExample.Opened);
                outputItem.Published.Should().Be(videoExample.Published);
                outputItem.Duration.Should().Be(videoExample.Duration);
                outputItem.Rating.Should().Be(videoExample.Rating.ToStringSignal());
                outputItem.ThumbFileUrl.Should().Be(videoExample.Thumb!.Path);
                outputItem.ThumbHalfFileUrl.Should().Be(videoExample.Thumb.Path);
                outputItem.BannerFileUrl.Should().Be(videoExample.Banner!.Path);
                outputItem.VideoFileUrl.Should().Be(videoExample.Media!.FilePath);
                outputItem.TrailerFileUrl.Should().Be(videoExample.Trailer!.FilePath);

            });
            _videoRepositoryMock.VerifyAll();
        }

        [Fact(DisplayName = nameof(ListVideosWithRelations))]
        [Trait("Application", "ListVideos - Use Cases")]
        public async Task ListVideosWithRelations()
        {
            var (exampleVideoList, exampleCategoryList, exampleGenresList) = _fixture.GetExampleVideoListWithRelations();
            var exampleCategoriesIds = exampleCategoryList.Select(x => x.Id).ToList();
            var exampleGenresIds = exampleGenresList.Select(x => x.Id).ToList();

            var outputRepositorySearch = new SearchOutput<DomainEntity.Video>(
               currentPage: 1,
               perPage: 10,
               items: exampleVideoList,
               total: exampleVideoList.Count
               );

            var input = new ListVideosInput(
                1,
                10,
                "",
                "",
                SearchOrder.Asc
                );

            _categoryRepositoryMock.Setup(x => x.GetListByIds(
                It.Is<List<Guid>>(list => list.All(exampleCategoriesIds.Contains)
                && list.Count == exampleCategoriesIds.Count),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(exampleCategoryList);

            _genreRepositoryMock.Setup(x => x.GetListByIds(
                It.Is<List<Guid>>(list => list.All(exampleGenresIds.Contains)
                && list.Count == exampleGenresIds.Count),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(exampleGenresList);

            _videoRepositoryMock.Setup(x => x.Search(
                It.Is<SearchInput>(x =>
                    x.Page == input.Page &&
                    x.PerPage == input.PerPage &&
                    x.Search == input.Search &&
                    x.OrderBy == input.Sort &&
                    x.Order == input.Dir),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(outputRepositorySearch);

            var output = await _useCases.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(exampleVideoList.Count);
            output.Items.Should().HaveCount(exampleVideoList.Count);
            output.Items.ToList().ForEach(outputItem =>
            {
                var videoExample = exampleVideoList.Find(x => x.Id == outputItem.Id);

                videoExample.Should().NotBeNull();
                outputItem.Should().NotBeNull();
                outputItem.Id.Should().Be(videoExample!.Id);
                outputItem.Title.Should().Be(videoExample.Title);
                outputItem.Description.Should().Be(videoExample.Description);
                outputItem.YearLauched.Should().Be(videoExample.YearLauched);
                outputItem.Opened.Should().Be(videoExample.Opened);
                outputItem.Published.Should().Be(videoExample.Published);
                outputItem.Duration.Should().Be(videoExample.Duration);
                outputItem.Rating.Should().Be(videoExample.Rating.ToStringSignal());
                outputItem.ThumbFileUrl.Should().Be(videoExample.Thumb!.Path);
                outputItem.ThumbHalfFileUrl.Should().Be(videoExample.Thumb.Path);
                outputItem.BannerFileUrl.Should().Be(videoExample.Banner!.Path);
                outputItem.VideoFileUrl.Should().Be(videoExample.Media!.FilePath);
                outputItem.TrailerFileUrl.Should().Be(videoExample.Trailer!.FilePath);
                outputItem.Categories.ToList().ForEach(relation =>
                {
                    var exampleCategory = exampleCategoryList.Find(x => x.Id == relation.Id);
                    exampleCategory.Should().NotBeNull();
                    relation.Name.Should().Be(exampleCategory!.Name);
                });

                outputItem.Genres.ToList().ForEach(relation =>
                {
                    var exampleGenre = exampleGenresList.Find(x => x.Id == relation.Id);
                    exampleGenre.Should().NotBeNull();
                    relation.Name.Should().Be(exampleGenre!.Name);
                });

                var outputItemGenres = outputItem.Genres
                    .Select(dto => dto.Id).ToList();
                outputItemGenres.Should().BeEquivalentTo(videoExample.Genres);

                var outputItemCastMembers = outputItem.CastMembers
                    .Select(dto => dto.Id).ToList();
                outputItemCastMembers.Should().BeEquivalentTo(videoExample.CastMembers);

            });
            _videoRepositoryMock.VerifyAll();
            _categoryRepositoryMock.VerifyAll();
        }

        [Fact(DisplayName = nameof(ListVideosWithoutRelationsDoesCallRepositories))]
        [Trait("Application", "ListVideos - Use Cases")]
        public async Task ListVideosWithoutRelationsDoesCallRepositories()
        {
            var exampleVideo = _fixture.GetExampleVideoListWithoutRelations();


            var outputRepositorySearch = new SearchOutput<DomainEntity.Video>(
               currentPage: 1,
               perPage: 10,
               items: exampleVideo,
               total: exampleVideo.Count
               );

            var input = new ListVideosInput(
                1,
                10,
                "",
                "",
                SearchOrder.Asc
                );

            _videoRepositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(x =>
                x.Page == input.Page &&
                x.PerPage == input.PerPage &&
                x.Search == input.Search &&
                x.OrderBy == input.Sort &&
                x.Order == input.Dir),
            It.IsAny<CancellationToken>()
            )).ReturnsAsync(outputRepositorySearch);

            var output = await _useCases.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(exampleVideo.Count);
            output.Items.Should().HaveCount(exampleVideo.Count);
            output.Items.ToList().ForEach(outputItem =>
            {
                var videoExample = exampleVideo.Find(x => x.Id == outputItem.Id);

                videoExample.Should().NotBeNull();
                outputItem.Should().NotBeNull();
                outputItem.Id.Should().Be(videoExample!.Id);
                outputItem.Title.Should().Be(videoExample.Title);
                outputItem.Categories.Should().HaveCount(0);
                outputItem.Genres.Should().HaveCount(0);
                outputItem.CastMembers.Should().HaveCount(0);
            });
            _videoRepositoryMock.VerifyAll();
            _categoryRepositoryMock.Verify(x => x.GetListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
                ), Times.Never);
            _genreRepositoryMock.Verify(x => x.GetListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
                ), Times.Never);
        }

        [Fact(DisplayName = nameof(ReturnsEmpty))]
        [Trait("Application", "ListVideos - Use Cases")]
        public async Task ReturnsEmpty()
        {
            var exampleVideoList = new List<DomainEntity.Video>();
            var outputRepositorySearch = new SearchOutput<DomainEntity.Video>(
               currentPage: 1,
               perPage: 10,
               items: exampleVideoList,
               total: exampleVideoList.Count
               );
            var input = new ListVideosInput(
                1,
                10,
                "",
                "",
                SearchOrder.Asc
                );

            _videoRepositoryMock.Setup(x => x.Search(
                It.Is<SearchInput>(x =>
                    x.Page == input.Page &&
                    x.PerPage == input.PerPage &&
                    x.Search == input.Search &&
                    x.OrderBy == input.Sort &&
                    x.Order == input.Dir),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(outputRepositorySearch);

            var output = await _useCases.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(exampleVideoList.Count);
            output.Items.Should().HaveCount(exampleVideoList.Count);

            _videoRepositoryMock.VerifyAll();
        }
    }
}