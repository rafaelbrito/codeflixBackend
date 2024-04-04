using FC.Codeflix.Catalog.Application.Inferfaces;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FluentAssertions;
using Moq;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo;
using FC.Codeflix.Catalog.Domain.Extensions;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo;
using System;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Application.Common;

namespace FC.Codeflix.Catalog.UniTests.Application.Video.UpdateVideo
{
    [Collection(nameof(UpdateVideoTestFixture))]
    public class UpdateVideoTest
    {
        private readonly UpdateVideoTestFixture _fixture;
        private readonly Mock<IVideoRepository> _videoRepositoryMock;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<IGenreRepository> _genreRepositoryMock;
        private readonly Mock<ICastMemberRepository> _castMemberRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IStorageService> _storageServiceMock;
        private readonly UseCase.UpdateVideo _useCases;

        public UpdateVideoTest(UpdateVideoTestFixture fixture)
        {
            _fixture = fixture;
            _videoRepositoryMock = new Mock<IVideoRepository>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _genreRepositoryMock = new Mock<IGenreRepository>();
            _castMemberRepositoryMock = new Mock<ICastMemberRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _storageServiceMock = new Mock<IStorageService>();

            _useCases = new UseCase.UpdateVideo(
                _videoRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _genreRepositoryMock.Object,
                _castMemberRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _storageServiceMock.Object
                );
        }

        [Fact(DisplayName = nameof(UpdateBasicInfoVideo))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateBasicInfoVideo()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var input = _fixture.CreateValidInput(exampleVideo.Id);

            _videoRepositoryMock.Setup(repository => repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleVideo);

            var output = await _useCases.Handle(input, CancellationToken.None);

            _unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()));

            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLauched.Should().Be(input.YearLauched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
            output.CreatedAt.Should().NotBeSameDateAs(default);


            _videoRepositoryMock.VerifyAll();

            _videoRepositoryMock.Verify(x => x.Update(
                It.Is<DomainEntity.Video>(video =>
                   video.Id == exampleVideo.Id
                && video.Title == input.Title
                && video.Description == input.Description
                && video.YearLauched == input.YearLauched
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.Rating == input.Rating
                && video.Duration == input.Duration),
                It.IsAny<CancellationToken>()
                ), Times.Once);
        }

        [Fact(DisplayName = nameof(UpdateVideosWithCategoriesIds))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideosWithCategoriesIds()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var exampleCategoriesIds = _fixture.GetListRandomIds();
            var input = _fixture.CreateValidInput(exampleVideo.Id, categoriesIds: exampleCategoriesIds);

            _videoRepositoryMock.Setup(repository => repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleVideo);

            _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.Is<List<Guid>>(idsList =>
                    idsList.Count == exampleCategoriesIds.Count
                    && idsList.All(id => exampleCategoriesIds.Contains(id))
                    ),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleCategoriesIds);


            var output = await _useCases.Handle(input, CancellationToken.None);

            _unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()));

            _videoRepositoryMock.VerifyAll();
            _categoryRepositoryMock.VerifyAll();

            _videoRepositoryMock.Verify(x => x.Update(
                It.Is<DomainEntity.Video>(video =>
                   video.Id == exampleVideo.Id
                && video.Title == input.Title
                && video.Description == input.Description
                && video.YearLauched == input.YearLauched
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.Rating == input.Rating
                && video.Duration == input.Duration
                && video.Categories.All(categoryId => exampleCategoriesIds.Contains(categoryId)
                && video.Categories.Count == exampleCategoriesIds.Count)),
                It.IsAny<CancellationToken>()
                ), Times.Once);

            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLauched.Should().Be(input.YearLauched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
            output.CreatedAt.Should().NotBeSameDateAs(default);
            output.Categories.Select(category => category.Id).ToList()
                .Should().BeEquivalentTo(exampleCategoriesIds);
        }

        [Fact(DisplayName = nameof(UpdateVideosWithGenresIds))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideosWithGenresIds()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var exampleGenresIds = _fixture.GetListRandomIds();
            var input = _fixture.CreateValidInput(exampleVideo.Id, exampleGenresIds);

            _videoRepositoryMock.Setup(repository => repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleVideo);

            _genreRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.Is<List<Guid>>(idsList =>
                    idsList.Count == exampleGenresIds.Count
                    && idsList.All(id => exampleGenresIds.Contains(id))
                    ),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleGenresIds);


            var output = await _useCases.Handle(input, CancellationToken.None);

            _unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()));

            _videoRepositoryMock.VerifyAll();
            _genreRepositoryMock.VerifyAll();

            _videoRepositoryMock.Verify(x => x.Update(
                It.Is<DomainEntity.Video>(video =>
                   video.Id == exampleVideo.Id
                && video.Title == input.Title
                && video.Description == input.Description
                && video.YearLauched == input.YearLauched
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.Rating == input.Rating
                && video.Duration == input.Duration
                && video.Genres.All(genreId => exampleGenresIds.Contains(genreId)
                && video.Genres.Count == exampleGenresIds.Count)),
                It.IsAny<CancellationToken>()
                ), Times.Once);

            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLauched.Should().Be(input.YearLauched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
            output.CreatedAt.Should().NotBeSameDateAs(default);
            output.Genres.Select(genre => genre.Id).ToList()
                .Should().BeEquivalentTo(exampleGenresIds);
        }

        [Fact(DisplayName = nameof(UpdateVideosWithCastMembersIds))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideosWithCastMembersIds()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var exampleCastMembersIds = _fixture.GetListRandomIds();
            var input = _fixture.CreateValidInput(exampleVideo.Id, castMembersIds: exampleCastMembersIds);

            _videoRepositoryMock.Setup(repository => repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleVideo);

            _castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.Is<List<Guid>>(idsList =>
                    idsList.Count == exampleCastMembersIds.Count
                    && idsList.All(id => exampleCastMembersIds.Contains(id))
                    ),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleCastMembersIds);


            var output = await _useCases.Handle(input, CancellationToken.None);

            _unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()));

            _videoRepositoryMock.VerifyAll();
            _castMemberRepositoryMock.VerifyAll();

            _videoRepositoryMock.Verify(x => x.Update(
                It.Is<DomainEntity.Video>(video =>
                   video.Id == exampleVideo.Id
                && video.Title == input.Title
                && video.Description == input.Description
                && video.YearLauched == input.YearLauched
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.Rating == input.Rating
                && video.Duration == input.Duration
                && video.CastMembers.All(castMembersId => exampleCastMembersIds.Contains(castMembersId)
                && video.CastMembers.Count == exampleCastMembersIds.Count)),
                It.IsAny<CancellationToken>()
                ), Times.Once);

            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLauched.Should().Be(input.YearLauched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
            output.CreatedAt.Should().NotBeSameDateAs(default);
            output.CastMembers.Select(genre => genre.Id).ToList()
                .Should().BeEquivalentTo(exampleCastMembersIds);
        }

        [Fact(DisplayName = nameof(UpdateVideosWithoutRelationsWithRelations))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideosWithoutRelationsWithRelations()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var exampleGenresIds = _fixture.GetListRandomIds();
            var exampleCastMembersIds = _fixture.GetListRandomIds();
            var exampleCategoriesIds = _fixture.GetListRandomIds();
            var input = _fixture.CreateValidInput(exampleVideo.Id, exampleGenresIds, exampleCategoriesIds, exampleCastMembersIds);

            _videoRepositoryMock.Setup(repository => repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleVideo);

            _genreRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.Is<List<Guid>>(idsList =>
                    idsList.Count == exampleGenresIds.Count
                    && idsList.All(id => exampleGenresIds.Contains(id))
                    ),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleGenresIds);

            _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.Is<List<Guid>>(idsList =>
                    idsList.Count == exampleCategoriesIds.Count
                    && idsList.All(id => exampleCategoriesIds.Contains(id))
                    ),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleCategoriesIds);

            _castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.Is<List<Guid>>(idsList =>
                    idsList.Count == exampleCastMembersIds.Count
                    && idsList.All(id => exampleCastMembersIds.Contains(id))
                    ),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleCastMembersIds);


            var output = await _useCases.Handle(input, CancellationToken.None);

            _unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()));

            _videoRepositoryMock.VerifyAll();
            _genreRepositoryMock.VerifyAll();
            _categoryRepositoryMock.VerifyAll();
            _castMemberRepositoryMock.VerifyAll();

            _videoRepositoryMock.Verify(x => x.Update(
                It.Is<DomainEntity.Video>(video =>
                   video.Id == exampleVideo.Id
                && video.Title == input.Title
                && video.Description == input.Description
                && video.YearLauched == input.YearLauched
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.Rating == input.Rating
                && video.Duration == input.Duration
                && video.Genres.All(genreId => exampleGenresIds.Contains(genreId))
                && video.Genres.Count == exampleGenresIds.Count
                && video.Categories.All(categoryId => exampleCategoriesIds.Contains(categoryId))
                && video.Categories.Count == exampleCategoriesIds.Count
                && video.CastMembers.All(castMemberId => exampleCastMembersIds.Contains(castMemberId))
                && video.CastMembers.Count == exampleCastMembersIds.Count
                ),
                It.IsAny<CancellationToken>()
                ), Times.Once);

            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLauched.Should().Be(input.YearLauched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
            output.CreatedAt.Should().NotBeSameDateAs(default);
            output.Genres.Select(genre => genre.Id).ToList()
                .Should().BeEquivalentTo(exampleGenresIds);
            output.Categories.Select(category => category.Id).ToList()
              .Should().BeEquivalentTo(exampleCategoriesIds);
            output.CastMembers.Select(castMember => castMember.Id).ToList()
              .Should().BeEquivalentTo(exampleCastMembersIds);
        }

        [Fact(DisplayName = nameof(UpdateVideosWithRelations))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideosWithRelations()
        {
            var exampleVideo = _fixture.GetValidVideoWithAllProperties();
            var exampleGenresIds = _fixture.GetListRandomIds();
            var exampleCastMembersIds = _fixture.GetListRandomIds();
            var exampleCategoriesIds = _fixture.GetListRandomIds();
            var input = _fixture.CreateValidInput(exampleVideo.Id, exampleGenresIds, exampleCategoriesIds, exampleCastMembersIds);

            _videoRepositoryMock.Setup(repository => repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleVideo);

            _genreRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.Is<List<Guid>>(idsList =>
                    idsList.Count == exampleGenresIds.Count
                    && idsList.All(id => exampleGenresIds.Contains(id))
                    ),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleGenresIds);

            _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.Is<List<Guid>>(idsList =>
                    idsList.Count == exampleCategoriesIds.Count
                    && idsList.All(id => exampleCategoriesIds.Contains(id))
                    ),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleCategoriesIds);

            _castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.Is<List<Guid>>(idsList =>
                    idsList.Count == exampleCastMembersIds.Count
                    && idsList.All(id => exampleCastMembersIds.Contains(id))
                    ),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleCastMembersIds);


            var output = await _useCases.Handle(input, CancellationToken.None);

            _unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()));

            _videoRepositoryMock.VerifyAll();
            _genreRepositoryMock.VerifyAll();
            _categoryRepositoryMock.VerifyAll();
            _castMemberRepositoryMock.VerifyAll();

            _videoRepositoryMock.Verify(x => x.Update(
                It.Is<DomainEntity.Video>(video =>
                   video.Id == exampleVideo.Id
                && video.Title == input.Title
                && video.Description == input.Description
                && video.YearLauched == input.YearLauched
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.Rating == input.Rating
                && video.Duration == input.Duration
                && video.Genres.All(genreId => exampleGenresIds.Contains(genreId))
                && video.Genres.Count == exampleGenresIds.Count
                && video.Categories.All(categoryId => exampleCategoriesIds.Contains(categoryId))
                && video.Categories.Count == exampleCategoriesIds.Count
                && video.CastMembers.All(castMemberId => exampleCastMembersIds.Contains(castMemberId))
                && video.CastMembers.Count == exampleCastMembersIds.Count
                ),
                It.IsAny<CancellationToken>()
                ), Times.Once);

            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLauched.Should().Be(input.YearLauched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
            output.CreatedAt.Should().NotBeSameDateAs(default);
            output.Genres.Select(genre => genre.Id).ToList()
                .Should().BeEquivalentTo(exampleGenresIds);
            output.Categories.Select(category => category.Id).ToList()
              .Should().BeEquivalentTo(exampleCategoriesIds);
            output.CastMembers.Select(castMember => castMember.Id).ToList()
              .Should().BeEquivalentTo(exampleCastMembersIds);
        }

        [Fact(DisplayName = nameof(UpdateVideosRemoveRelations))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideosRemoveRelations()
        {
            var exampleVideo = _fixture.GetValidVideoWithAllProperties();

            var input = _fixture.CreateValidInput(exampleVideo.Id, new(), new(), new());

            _videoRepositoryMock.Setup(repository => repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleVideo);


            var output = await _useCases.Handle(input, CancellationToken.None);

            _unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()));

            _videoRepositoryMock.VerifyAll();

            _genreRepositoryMock.Verify(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()),
                Times.Never);

            _categoryRepositoryMock.Verify(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()),
                Times.Never);

            _castMemberRepositoryMock.Verify(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()),
                Times.Never);

            _videoRepositoryMock.Verify(x => x.Update(
                It.Is<DomainEntity.Video>(video =>
                   video.Id == exampleVideo.Id
                && video.Title == input.Title
                && video.Description == input.Description
                && video.YearLauched == input.YearLauched
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.Rating == input.Rating
                && video.Duration == input.Duration
                && video.Genres.All(genreId => exampleVideo.Genres.Contains(genreId))
                && video.Genres.Count == exampleVideo.Genres.Count
                && video.Categories.All(categoryId => exampleVideo.Categories.Contains(categoryId))
                && video.Categories.Count == exampleVideo.Categories.Count
                && video.CastMembers.All(castMemberId => exampleVideo.CastMembers.Contains(castMemberId))
                && video.CastMembers.Count == exampleVideo.CastMembers.Count
                ),
                It.IsAny<CancellationToken>()
                ), Times.Once);

            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLauched.Should().Be(input.YearLauched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
            output.CreatedAt.Should().NotBeSameDateAs(default);
            output.Genres.Select(genre => genre.Id).ToList()
                 .Should().BeEquivalentTo(exampleVideo.Genres);
            output.Categories.Select(category => category.Id).ToList()
              .Should().BeEquivalentTo(exampleVideo.Categories);
            output.CastMembers.Select(castMember => castMember.Id).ToList()
              .Should().BeEquivalentTo(exampleVideo.CastMembers);
        }

        [Fact(DisplayName = nameof(UpdateVideoInsertBanner))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideoInsertBanner()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var input = _fixture.CreateValidInput(exampleVideo.Id, banner: _fixture.GetValidImageFileInput());
            var bannerPath = $"storage/banner.{input.Banner}";

            _videoRepositoryMock.Setup(repository => repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleVideo);

            _storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(name => name == StorageFileName.Create(
                    exampleVideo.Id,
                    nameof(exampleVideo.Banner),
                    input.Banner!.Extension)),
                It.IsAny<MemoryStream>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(bannerPath);

            var output = await _useCases.Handle(input, CancellationToken.None);

            _unitOfWorkMock.Verify(x => x.Commit(
           It.IsAny<CancellationToken>()));

            _videoRepositoryMock.VerifyAll();
            _storageServiceMock.VerifyAll();

            _videoRepositoryMock.Verify(x => x.Update(
                It.Is<DomainEntity.Video>(video =>
                   video.Id == exampleVideo.Id
                && video.Title == input.Title
                && video.Description == input.Description
                && video.YearLauched == input.YearLauched
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.Rating == input.Rating
                && video.Duration == input.Duration
                && video.Banner!.Path == bannerPath
                ),
                It.IsAny<CancellationToken>()
                ), Times.Once);

            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLauched.Should().Be(input.YearLauched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
            output.CreatedAt.Should().NotBeSameDateAs(default);
            output.BannerFileUrl.Should().Be(bannerPath);

        }

        [Fact(DisplayName = nameof(UpdateVideoKeepBannerWhenReciveNull))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideoKeepBannerWhenReciveNull()
        {
            var exampleVideo = _fixture.GetValidVideoWithAllProperties();
            var input = _fixture.CreateValidInput(exampleVideo.Id, banner: null);
            var bannerPath = $"storage/banner.{input.Banner}";

            _videoRepositoryMock.Setup(repository => repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleVideo);

            var output = await _useCases.Handle(input, CancellationToken.None);

            _unitOfWorkMock.Verify(x => x.Commit(
           It.IsAny<CancellationToken>()));

            _videoRepositoryMock.VerifyAll();
            _storageServiceMock.Verify(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
                ), Times.Never);

            _videoRepositoryMock.Verify(x => x.Update(
                It.Is<DomainEntity.Video>(video =>
                   video.Id == exampleVideo.Id
                && video.Title == input.Title
                && video.Description == input.Description
                && video.YearLauched == input.YearLauched
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.Rating == input.Rating
                && video.Duration == input.Duration
                && video.Banner!.Path == exampleVideo.Banner!.Path
                ),
                It.IsAny<CancellationToken>()
                ), Times.Once);

            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLauched.Should().Be(input.YearLauched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
            output.CreatedAt.Should().NotBeSameDateAs(default);
            output.BannerFileUrl.Should().Be(exampleVideo.Banner!.Path);

        }

        [Fact(DisplayName = nameof(UpdateVideoInsertThumb))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideoInsertThumb()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var input = _fixture.CreateValidInput(exampleVideo.Id, thumb: _fixture.GetValidImageFileInput());
            var thumbPath = $"storage/thumb.{input.Thumb}";

            _videoRepositoryMock.Setup(repository => repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleVideo);

            _storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(name => name == StorageFileName.Create(
                    exampleVideo.Id,
                    nameof(exampleVideo.Thumb),
                    input.Thumb!.Extension)),
                It.IsAny<MemoryStream>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(thumbPath);

            var output = await _useCases.Handle(input, CancellationToken.None);

            _unitOfWorkMock.Verify(x => x.Commit(
           It.IsAny<CancellationToken>()));

            _videoRepositoryMock.VerifyAll();
            _storageServiceMock.VerifyAll();

            _videoRepositoryMock.Verify(x => x.Update(
                It.Is<DomainEntity.Video>(video =>
                   video.Id == exampleVideo.Id
                && video.Title == input.Title
                && video.Description == input.Description
                && video.YearLauched == input.YearLauched
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.Rating == input.Rating
                && video.Duration == input.Duration
                && video.Thumb!.Path == thumbPath
                ),
                It.IsAny<CancellationToken>()
                ), Times.Once);

            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLauched.Should().Be(input.YearLauched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
            output.CreatedAt.Should().NotBeSameDateAs(default);
            output.ThumbFileUrl.Should().Be(thumbPath);

        }

        [Fact(DisplayName = nameof(UpdateVideoKeepThumbWhenReciveNull))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideoKeepThumbWhenReciveNull()
        {
            var exampleVideo = _fixture.GetValidVideoWithAllProperties();
            var input = _fixture.CreateValidInput(exampleVideo.Id, thumb: null);
            var thumbPath = $"storage/thumb.{input.Thumb}";

            _videoRepositoryMock.Setup(repository => repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleVideo);

            var output = await _useCases.Handle(input, CancellationToken.None);

            _unitOfWorkMock.Verify(x => x.Commit(
           It.IsAny<CancellationToken>()));

            _videoRepositoryMock.VerifyAll();
            _storageServiceMock.Verify(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
                ), Times.Never);

            _videoRepositoryMock.Verify(x => x.Update(
                It.Is<DomainEntity.Video>(video =>
                   video.Id == exampleVideo.Id
                && video.Title == input.Title
                && video.Description == input.Description
                && video.YearLauched == input.YearLauched
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.Rating == input.Rating
                && video.Duration == input.Duration
                && video.Thumb!.Path == exampleVideo.Thumb!.Path
                ),
                It.IsAny<CancellationToken>()
                ), Times.Once);

            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLauched.Should().Be(input.YearLauched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
            output.CreatedAt.Should().NotBeSameDateAs(default);
            output.ThumbFileUrl.Should().Be(exampleVideo.Thumb!.Path);

        }

        [Fact(DisplayName = nameof(UpdateVideoInsertThumbHalf))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideoInsertThumbHalf()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var input = _fixture.CreateValidInput(exampleVideo.Id, thumbHalf: _fixture.GetValidImageFileInput());
            var thumbPathHalf = $"storage/thumbhalf.{input.ThumbHalf}";

            _videoRepositoryMock.Setup(repository => repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleVideo);

            _storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(name => name == StorageFileName.Create(
                    exampleVideo.Id,
                    nameof(exampleVideo.ThumbHalf),
                    input.ThumbHalf!.Extension)),
                It.IsAny<MemoryStream>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(thumbPathHalf);

            var output = await _useCases.Handle(input, CancellationToken.None);

            _unitOfWorkMock.Verify(x => x.Commit(
           It.IsAny<CancellationToken>()));

            _videoRepositoryMock.VerifyAll();
            _storageServiceMock.VerifyAll();

            _videoRepositoryMock.Verify(x => x.Update(
                It.Is<DomainEntity.Video>(video =>
                   video.Id == exampleVideo.Id
                && video.Title == input.Title
                && video.Description == input.Description
                && video.YearLauched == input.YearLauched
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.Rating == input.Rating
                && video.Duration == input.Duration
                && video.ThumbHalf!.Path == thumbPathHalf
                ),
                It.IsAny<CancellationToken>()
                ), Times.Once);

            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLauched.Should().Be(input.YearLauched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
            output.CreatedAt.Should().NotBeSameDateAs(default);
            output.ThumbHalfFileUrl.Should().Be(thumbPathHalf);

        }

        [Fact(DisplayName = nameof(UpdateVideoKeepThumbHalfWhenReciveNull))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideoKeepThumbHalfWhenReciveNull()
        {
            var exampleVideo = _fixture.GetValidVideoWithAllProperties();
            var input = _fixture.CreateValidInput(exampleVideo.Id, thumbHalf: null);
            var thumbHalfPath = $"storage/thumbhalf.{input.ThumbHalf}";

            _videoRepositoryMock.Setup(repository => repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleVideo);

            var output = await _useCases.Handle(input, CancellationToken.None);

            _unitOfWorkMock.Verify(x => x.Commit(
           It.IsAny<CancellationToken>()));

            _videoRepositoryMock.VerifyAll();
            _storageServiceMock.Verify(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
                ), Times.Never);

            _videoRepositoryMock.Verify(x => x.Update(
                It.Is<DomainEntity.Video>(video =>
                   video.Id == exampleVideo.Id
                && video.Title == input.Title
                && video.Description == input.Description
                && video.YearLauched == input.YearLauched
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.Rating == input.Rating
                && video.Duration == input.Duration
                && video.ThumbHalf!.Path == exampleVideo.ThumbHalf!.Path
                ),
                It.IsAny<CancellationToken>()
                ), Times.Once);

            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLauched.Should().Be(input.YearLauched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
            output.CreatedAt.Should().NotBeSameDateAs(default);
            output.ThumbHalfFileUrl.Should().Be(exampleVideo.ThumbHalf!.Path);

        }

        [Fact(DisplayName = nameof(ThrowsInvalidGenreId))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task ThrowsInvalidGenreId()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var exampleGenresIds = _fixture.GetListRandomIds();
            var invalidGenreId = Guid.NewGuid();
            var inputInvalidList = exampleGenresIds.Concat(new List<Guid>() { invalidGenreId }).ToList();
            var input = _fixture.CreateValidInput(exampleVideo.Id, inputInvalidList);

            _videoRepositoryMock.Setup(repository => repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleVideo);

            _genreRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleGenresIds);


            var action = () => _useCases.Handle(input, CancellationToken.None);
            await action.Should().ThrowAsync<RelatedAggregateException>()
                .WithMessage($"Related genres id (or ids) not found: '{invalidGenreId}'");

            _videoRepositoryMock.VerifyAll();
            _genreRepositoryMock.VerifyAll();

            _unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = nameof(ThrowsInvalidCategoryId))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task ThrowsInvalidCategoryId()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var exampleCategoriesIds = _fixture.GetListRandomIds();
            var invalidCategoryId = Guid.NewGuid();
            var inputInvalidList = exampleCategoriesIds.Concat(new List<Guid>() { invalidCategoryId }).ToList();
            var input = _fixture.CreateValidInput(exampleVideo.Id, categoriesIds: inputInvalidList);

            _videoRepositoryMock.Setup(repository => repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleVideo);

            _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleCategoriesIds);


            var action = () => _useCases.Handle(input, CancellationToken.None);
            await action.Should().ThrowAsync<RelatedAggregateException>()
                .WithMessage($"Related category id (or ids) not found: '{invalidCategoryId}'");

            _videoRepositoryMock.VerifyAll();
            _castMemberRepositoryMock.VerifyAll();

            _unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = nameof(ThrowsInvalidCastMemberId))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task ThrowsInvalidCastMemberId()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var exampleCastMembersIds = _fixture.GetListRandomIds();
            var invalidCastMemberId = Guid.NewGuid();
            var inputInvalidList = exampleCastMembersIds.Concat(new List<Guid>() { invalidCastMemberId }).ToList();
            var input = _fixture.CreateValidInput(exampleVideo.Id, castMembersIds: inputInvalidList);

            _videoRepositoryMock.Setup(repository => repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleVideo);

            _castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleCastMembersIds);


            var action = () => _useCases.Handle(input, CancellationToken.None);
            await action.Should().ThrowAsync<RelatedAggregateException>()
                .WithMessage($"Related castmembers id (or ids) not found: '{invalidCastMemberId}'");

            _videoRepositoryMock.VerifyAll();
            _castMemberRepositoryMock.VerifyAll();

            _unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = nameof(ThrowsVideoNotFound))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task ThrowsVideoNotFound()
        {
            var input = _fixture.CreateValidInput(Guid.NewGuid());

            _videoRepositoryMock.Setup(repository => repository.Get(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
                )).ThrowsAsync(new NotFoundException("Video not found"));

            var action = () => _useCases.Handle(input, CancellationToken.None);
            await action.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Video not found");

            _videoRepositoryMock.VerifyAll();

            _videoRepositoryMock.Verify(x => x.Update(
                It.IsAny<DomainEntity.Video>(),
                It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Theory(DisplayName = nameof(ThrowsUpdateVideoInvalidInput))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        [ClassData(typeof(UpdateVideoTestDataGenerator))]
        public async Task ThrowsUpdateVideoInvalidInput(
            UpdateVideoInput input,
            string exceptionMessage
            )
        {
            var exampleVideo = _fixture.GetValidVideo();

            _videoRepositoryMock.Setup(repository => repository.Get(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(exampleVideo);

            var action = () => _useCases.Handle(input, CancellationToken.None);
            var exception = await action.Should()
                .ThrowAsync<EntityValidationException>()
                .WithMessage("There are validation errors");

            exception.Which.Erros!.ToList()[0]
                .Message.Should().Be(exceptionMessage);

            _videoRepositoryMock.Verify(x => x.Update(
                It.IsAny<DomainEntity.Video>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
