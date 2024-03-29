using FC.Codeflix.Catalog.Application.Inferfaces;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using FluentAssertions;
using Moq;
using Xunit;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Extensions;

namespace FC.Codeflix.Catalog.UniTests.Application.Video.CreateVideo
{
    [Collection(nameof(CreateVideoTestFixture))]
    public class CreateVideoTest
    {
        private readonly CreateVideoTestFixture _fixture;
        public CreateVideoTest(CreateVideoTestFixture fixture)
            => _fixture = fixture;


        [Fact(DisplayName = (nameof(Create)))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task Create()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var repositoryCategoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMembersRepositoryMock = new Mock<ICastMemberRepository>();
            var storageMock = new Mock<IStorageService>();
            var useCase = new UseCase.CreateVideo(
                repositoryMock.Object,
                repositoryCategoryMock.Object,
                genreRepositoryMock.Object,
                castMembersRepositoryMock.Object,
                unitOfWork.Object,
                storageMock.Object
                    );
            var input = _fixture.GetValidVideoInput();
            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(x => x.Insert(
                It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty &&
                video.Title == input.Title &&
                video.Description == input.Description &&
                video.YearLauched == input.YearLauched &&
                video.Opened == input.Opened &&
                video.Published == input.Published &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating
                ),
                It.IsAny<CancellationToken>())
            );
            unitOfWork.Verify(x => x.Commit(
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
        }

        [Theory(DisplayName = (nameof(ThrowsWithInvalidInput)))]
        [Trait("Application", "CreateVideo - Use Cases")]
        [ClassData(typeof(CreateVideoTestDataGenerator))]
        public async Task ThrowsWithInvalidInput(
            CreateVideoInput input,
            string exceptionMessage
            )
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var repositoryCategoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMembersRepositoryMock = new Mock<ICastMemberRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var storageMock = new Mock<IStorageService>();
            var useCase = new UseCase.CreateVideo(
                repositoryMock.Object,
                repositoryCategoryMock.Object,
                genreRepositoryMock.Object,
                castMembersRepositoryMock.Object,
                unitOfWork.Object,
                storageMock.Object
                    );

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            var exception = await action.Should()
                .ThrowAsync<EntityValidationException>();

            exception.WithMessage("There are validation errors")
                .Which.Erros!.ToList()[0]
                .Message.Should().Be(exceptionMessage);

            repositoryMock.Verify(x => x.Insert(
                It.IsAny<DomainEntity.Video>(),
                It.IsAny<CancellationToken>()), Times.Never
            );
        }

        [Fact(DisplayName = (nameof(CreateWithCategories)))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task CreateWithCategories()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMembersRepositoryMock = new Mock<ICastMemberRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var storageMock = new Mock<IStorageService>();

            var exampleCategoriesIds = _fixture.GetListRandomIds(5);
            categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>())
                ).ReturnsAsync(exampleCategoriesIds);

            var useCase = new UseCase.CreateVideo(
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMembersRepositoryMock.Object,
                unitOfWork.Object,
                storageMock.Object
                    );

            var input = _fixture.GetValidVideoInput(exampleCategoriesIds);
            var output = await useCase.Handle(input, CancellationToken.None);

            unitOfWork.Verify(x => x.Commit(
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
            var outputItemCategories = output.Categories
                   .Select(dto => dto.Id).ToList();
            outputItemCategories.Should().BeEquivalentTo(exampleCategoriesIds);

            repositoryMock.Verify(x => x.Insert(
                It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty &&
                video.Title == input.Title &&
                video.Description == input.Description &&
                video.YearLauched == input.YearLauched &&
                video.Opened == input.Opened &&
                video.Published == input.Published &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating &&
                video.Categories.All(categoryId =>
                exampleCategoriesIds.Contains(categoryId)
                )),
                It.IsAny<CancellationToken>())
            );
            categoryRepositoryMock.VerifyAll();
        }

        [Fact(DisplayName = (nameof(ThrowsWhenCategoryIdInvalid)))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task ThrowsWhenCategoryIdInvalid()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMembersRepositoryMock = new Mock<ICastMemberRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var storageMock = new Mock<IStorageService>();

            var exampleCategoriesIds = _fixture.GetListRandomIds(5);
            var removeItem = exampleCategoriesIds[2];
            categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>())
                ).ReturnsAsync(exampleCategoriesIds
                .FindAll(x => x != removeItem)
                .AsReadOnly());


            var useCase = new UseCase.CreateVideo(
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMembersRepositoryMock.Object,
                unitOfWork.Object,
                storageMock.Object
                );
            var input = _fixture.GetValidVideoInput(exampleCategoriesIds);

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should()
              .ThrowAsync<RelatedAggregateException>()
              .WithMessage($"Related category id (or ids) not found: '{removeItem}'");
            categoryRepositoryMock.VerifyAll();
        }

        [Fact(DisplayName = (nameof(CreateWithGenres)))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task CreateWithGenres()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var genresRepositoryMock = new Mock<IGenreRepository>();
            var castMembersRepositoryMock = new Mock<ICastMemberRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var storageMock = new Mock<IStorageService>();

            var exampleListIds = _fixture.GetListRandomIds(5);
            genresRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>())
                ).ReturnsAsync(exampleListIds);

            var useCase = new UseCase.CreateVideo(
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genresRepositoryMock.Object,
                castMembersRepositoryMock.Object,
                unitOfWork.Object,
                storageMock.Object
                    );

            var input = _fixture.GetValidVideoInput(genresIds: exampleListIds);
            var output = await useCase.Handle(input, CancellationToken.None);

            unitOfWork.Verify(x => x.Commit(
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

            var outputItemGenres = output.Genres
                .Select(dto => dto.Id).ToList();
            outputItemGenres.Should().BeEquivalentTo(exampleListIds);


            repositoryMock.Verify(x => x.Insert(
                It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty &&
                video.Title == input.Title &&
                video.Description == input.Description &&
                video.YearLauched == input.YearLauched &&
                video.Opened == input.Opened &&
                video.Published == input.Published &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating &&
                video.Genres.All(id =>
                exampleListIds.Contains(id)
                )),
                It.IsAny<CancellationToken>())
            );
            genresRepositoryMock.VerifyAll();
        }

        [Fact(DisplayName = (nameof(ThrowsWhenGenresIdInvalid)))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task ThrowsWhenGenresIdInvalid()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMembersRepositoryMock = new Mock<ICastMemberRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var storageMock = new Mock<IStorageService>();

            var exampleGenres = _fixture.GetListRandomIds(5);
            var removeItem = exampleGenres[2];
            genreRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>())
                ).ReturnsAsync(exampleGenres
                .FindAll(x => x != removeItem)
                .AsReadOnly());


            var useCase = new UseCase.CreateVideo(
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMembersRepositoryMock.Object,
                unitOfWork.Object,
                storageMock.Object
                );
            var input = _fixture.GetValidVideoInput(genresIds: exampleGenres);

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should()
              .ThrowAsync<RelatedAggregateException>()
              .WithMessage($"Related genres id (or ids) not found: '{removeItem}'");
            genreRepositoryMock.VerifyAll();
        }

        [Fact(DisplayName = (nameof(CreateWithCastMembers)))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task CreateWithCastMembers()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var genresRepositoryMock = new Mock<IGenreRepository>();
            var castMembersRepositoryMock = new Mock<ICastMemberRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var storageMock = new Mock<IStorageService>();

            var exampleListIds = _fixture.GetListRandomIds(5);
            castMembersRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>())
                ).ReturnsAsync(exampleListIds);

            var useCase = new UseCase.CreateVideo(
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genresRepositoryMock.Object,
                castMembersRepositoryMock.Object,
                unitOfWork.Object,
                storageMock.Object
                    );

            var input = _fixture.GetValidVideoInput(castMembersIds: exampleListIds);
            var output = await useCase.Handle(input, CancellationToken.None);

            unitOfWork.Verify(x => x.Commit(
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
            output.Categories.Should().BeEmpty();
            output.Genres.Should().BeEmpty();
            var outputItemCastMember = output.CastMembers
                   .Select(dto => dto.Id).ToList();
            outputItemCastMember.Should().BeEquivalentTo(exampleListIds);

            repositoryMock.Verify(x => x.Insert(
                It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty &&
                video.Title == input.Title &&
                video.Description == input.Description &&
                video.YearLauched == input.YearLauched &&
                video.Opened == input.Opened &&
                video.Published == input.Published &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating &&
                video.CastMembers.All(id =>
                exampleListIds.Contains(id)
                )),
                It.IsAny<CancellationToken>())
            );
            castMembersRepositoryMock.VerifyAll();
        }

        [Fact(DisplayName = (nameof(ThrowsWhenCastMemberIdInvalid)))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task ThrowsWhenCastMemberIdInvalid()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMembersRepositoryMock = new Mock<ICastMemberRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var storageMock = new Mock<IStorageService>();

            var exampleGenres = _fixture.GetListRandomIds(5);
            var removeItem = exampleGenres[2];
            castMembersRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>())
                ).ReturnsAsync(exampleGenres
                .FindAll(x => x != removeItem)
                .AsReadOnly());


            var useCase = new UseCase.CreateVideo(
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMembersRepositoryMock.Object,
                unitOfWork.Object,
                storageMock.Object
                );
            var input = _fixture.GetValidVideoInput(castMembersIds: exampleGenres);

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should()
              .ThrowAsync<RelatedAggregateException>()
              .WithMessage($"Related castmembers id (or ids) not found: '{removeItem}'");
            castMembersRepositoryMock.VerifyAll();
        }

        [Fact(DisplayName = (nameof(CreateVideoWithThumb)))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task CreateVideoWithThumb()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var storageServiceMock = new Mock<IStorageService>();

            var expectedThumbName = "thumb.jpg";

            storageServiceMock.Setup(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(expectedThumbName);

            var repositoryCategoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMembersRepositoryMock = new Mock<ICastMemberRepository>();
            var useCase = new UseCase.CreateVideo(
                repositoryMock.Object,
                repositoryCategoryMock.Object,
                genreRepositoryMock.Object,
                castMembersRepositoryMock.Object,
                unitOfWork.Object,
                storageServiceMock.Object
                    );
            var input = _fixture.GetValidVideoInput(thumb: _fixture.GetValidImageFileInput());

            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(x => x.Insert(
                It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty &&
                video.Title == input.Title &&
                video.Description == input.Description &&
                video.YearLauched == input.YearLauched &&
                video.Opened == input.Opened &&
                video.Published == input.Published &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating
                ),
                It.IsAny<CancellationToken>())
            );
            unitOfWork.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()));
            storageServiceMock.VerifyAll();
            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLauched.Should().Be(input.YearLauched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
            output.CreatedAt.Should().NotBeSameDateAs(default);
            output.ThumbFileUrl.Should().Be(expectedThumbName);
        }

        [Fact(DisplayName = (nameof(CreateVideoWithBanner)))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task CreateVideoWithBanner()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var storageServiceMock = new Mock<IStorageService>();

            var expectedBannerName = "banner.jpg";

            storageServiceMock.Setup(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(expectedBannerName);

            var repositoryCategoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMembersRepositoryMock = new Mock<ICastMemberRepository>();
            var useCase = new UseCase.CreateVideo(
                repositoryMock.Object,
                repositoryCategoryMock.Object,
                genreRepositoryMock.Object,
                castMembersRepositoryMock.Object,
                unitOfWork.Object,
                storageServiceMock.Object
                    );
            var input = _fixture.GetValidVideoInput(banner: _fixture.GetValidImageFileInput());

            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(x => x.Insert(
                It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty &&
                video.Title == input.Title &&
                video.Description == input.Description &&
                video.YearLauched == input.YearLauched &&
                video.Opened == input.Opened &&
                video.Published == input.Published &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating
                ),
                It.IsAny<CancellationToken>())
            );
            unitOfWork.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()));
            storageServiceMock.VerifyAll();
            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLauched.Should().Be(input.YearLauched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
            output.CreatedAt.Should().NotBeSameDateAs(default);
            output.BannerFileUrl.Should().Be(expectedBannerName);
        }

        [Fact(DisplayName = (nameof(CreateVideoWithThumbHalf)))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task CreateVideoWithThumbHalf()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var storageServiceMock = new Mock<IStorageService>();

            var expectedThumbHalfName = "thumbhalf.jpg";

            storageServiceMock.Setup(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(expectedThumbHalfName);

            var repositoryCategoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMembersRepositoryMock = new Mock<ICastMemberRepository>();
            var useCase = new UseCase.CreateVideo(
                repositoryMock.Object,
                repositoryCategoryMock.Object,
                genreRepositoryMock.Object,
                castMembersRepositoryMock.Object,
                unitOfWork.Object,
                storageServiceMock.Object
                    );
            var input = _fixture.GetValidVideoInput(thumbHalf: _fixture.GetValidImageFileInput());

            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(x => x.Insert(
                It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty &&
                video.Title == input.Title &&
                video.Description == input.Description &&
                video.YearLauched == input.YearLauched &&
                video.Opened == input.Opened &&
                video.Published == input.Published &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating
                ),
                It.IsAny<CancellationToken>())
            );
            unitOfWork.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()));
            storageServiceMock.VerifyAll();
            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLauched.Should().Be(input.YearLauched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
            output.CreatedAt.Should().NotBeSameDateAs(default);
            output.ThumbHalfFileUrl.Should().Be(expectedThumbHalfName);
        }

        [Fact(DisplayName = (nameof(CreateVideoWithAllImages)))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task CreateVideoWithAllImages()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var storageServiceMock = new Mock<IStorageService>();

            var expectedBannerName = "banner.jpg";
            var expectedThumbName = "half.jpg";
            var expectedThumbHalfName = "thumbhalf.jpg";

            storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(x => x.EndsWith("-banner.jpg")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(expectedBannerName);

            storageServiceMock.Setup(x => x.Upload(
               It.Is<string>(x => x.EndsWith("-thumb.jpg")),
               It.IsAny<Stream>(),
               It.IsAny<CancellationToken>()
               )).ReturnsAsync(expectedThumbName);

            storageServiceMock.Setup(x => x.Upload(
               It.Is<string>(x => x.EndsWith("-thumbhalf.jpg")),
               It.IsAny<Stream>(),
               It.IsAny<CancellationToken>()
               )).ReturnsAsync(expectedThumbHalfName);

            var repositoryCategoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMembersRepositoryMock = new Mock<ICastMemberRepository>();
            var useCase = new UseCase.CreateVideo(
                repositoryMock.Object,
                repositoryCategoryMock.Object,
                genreRepositoryMock.Object,
                castMembersRepositoryMock.Object,
                unitOfWork.Object,
                storageServiceMock.Object
                    );
            var input = _fixture.GetValidVideoInputAllImages();

            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(x => x.Insert(
                It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty &&
                video.Title == input.Title &&
                video.Description == input.Description &&
                video.YearLauched == input.YearLauched &&
                video.Opened == input.Opened &&
                video.Published == input.Published &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating
                ),
                It.IsAny<CancellationToken>())
            );
            unitOfWork.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()));
            storageServiceMock.VerifyAll();
            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLauched.Should().Be(input.YearLauched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
            output.CreatedAt.Should().NotBeSameDateAs(default);
            output.BannerFileUrl.Should().Be(expectedBannerName);
            output.ThumbFileUrl.Should().Be(expectedThumbName);
            output.ThumbHalfFileUrl.Should().Be(expectedThumbHalfName);
        }

        [Fact(DisplayName = (nameof(CreateVideoWithMedia)))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task CreateVideoWithMedia()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var repositoryCategoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMembersRepositoryMock = new Mock<ICastMemberRepository>();

            var unitOfWork = new Mock<IUnitOfWork>();
            var storageServiceMock = new Mock<IStorageService>();

            var expectedMediaName = $"/storage/{_fixture.GetValidMediaPath()}";

            storageServiceMock.Setup(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(expectedMediaName);

            var useCase = new UseCase.CreateVideo(
                repositoryMock.Object,
                repositoryCategoryMock.Object,
                genreRepositoryMock.Object,
                castMembersRepositoryMock.Object,
                unitOfWork.Object,
                storageServiceMock.Object
                    );

            var input = _fixture.GetValidVideoInput(media: _fixture.GetValidMediaFileInput());

            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(x => x.Insert(
                It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty &&
                video.Title == input.Title &&
                video.Description == input.Description &&
                video.YearLauched == input.YearLauched &&
                video.Opened == input.Opened &&
                video.Published == input.Published &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating
                ),
                It.IsAny<CancellationToken>())
            );
            unitOfWork.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()));
            storageServiceMock.VerifyAll();
            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLauched.Should().Be(input.YearLauched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
            output.CreatedAt.Should().NotBeSameDateAs(default);
            output.VideoFileUrl.Should().Be(expectedMediaName);
        }

        [Fact(DisplayName = (nameof(CreateVideoWithTrailer)))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task CreateVideoWithTrailer()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var repositoryCategoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMembersRepositoryMock = new Mock<ICastMemberRepository>();

            var unitOfWork = new Mock<IUnitOfWork>();
            var storageServiceMock = new Mock<IStorageService>();

            var expectedMediaName = $"/storage/{_fixture.GetValidMediaPath()}";

            storageServiceMock.Setup(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(expectedMediaName);

            var useCase = new UseCase.CreateVideo(
                repositoryMock.Object,
                repositoryCategoryMock.Object,
                genreRepositoryMock.Object,
                castMembersRepositoryMock.Object,
                unitOfWork.Object,
                storageServiceMock.Object
                    );

            var input = _fixture.GetValidVideoInput(trailer: _fixture.GetValidMediaFileInput());

            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(x => x.Insert(
                It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty &&
                video.Title == input.Title &&
                video.Description == input.Description &&
                video.YearLauched == input.YearLauched &&
                video.Opened == input.Opened &&
                video.Published == input.Published &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating
                ),
                It.IsAny<CancellationToken>())
            );
            unitOfWork.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()));
            storageServiceMock.VerifyAll();
            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLauched.Should().Be(input.YearLauched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
            output.CreatedAt.Should().NotBeSameDateAs(default);
            output.TrailerFileUrl.Should().Be(expectedMediaName);
        }

        [Fact(DisplayName = (nameof(ThrowExceptionUploadErrorImage)))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task ThrowExceptionUploadErrorImage()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var storageServiceMock = new Mock<IStorageService>();

            storageServiceMock.Setup(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
                )).ThrowsAsync(new Exception("Something went wrong in upload"));

            var repositoryCategoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMembersRepositoryMock = new Mock<ICastMemberRepository>();
            var useCase = new UseCase.CreateVideo(
                repositoryMock.Object,
                repositoryCategoryMock.Object,
                genreRepositoryMock.Object,
                castMembersRepositoryMock.Object,
                unitOfWork.Object,
                storageServiceMock.Object
                    );
            var input = _fixture.GetValidVideoInputAllImages();

            var action = () => useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<Exception>()
                .WithMessage("Something went wrong in upload");
        }

        [Fact(DisplayName = (nameof(ThrowExceptionRollbackUploadErrorImage)))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task ThrowExceptionRollbackUploadErrorImage()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var storageServiceMock = new Mock<IStorageService>();


            storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(x => x.EndsWith("-banner.jpg")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync("123-banner.jpg");

            storageServiceMock.Setup(x => x.Upload(
               It.Is<string>(x => x.EndsWith("-thumb.jpg")),
               It.IsAny<Stream>(),
               It.IsAny<CancellationToken>()
               )).ReturnsAsync("123-thumb.jpg");

            storageServiceMock.Setup(x => x.Upload(
               It.Is<string>(x => x.EndsWith("-thumbhalf.jpg")),
               It.IsAny<Stream>(),
               It.IsAny<CancellationToken>()
               )).ThrowsAsync(new Exception("Something went wrong in upload"));

            var repositoryCategoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMembersRepositoryMock = new Mock<ICastMemberRepository>();
            var useCase = new UseCase.CreateVideo(
                repositoryMock.Object,
                repositoryCategoryMock.Object,
                genreRepositoryMock.Object,
                castMembersRepositoryMock.Object,
                unitOfWork.Object,
                storageServiceMock.Object
                    );
            var input = _fixture.GetValidVideoInputAllImages();

            var action = () => useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<Exception>()
                .WithMessage("Something went wrong in upload");
            storageServiceMock.Verify(x => x.Delete(
                It.Is<string>(x => (x == "123-banner.jpg") || (x == "123-thumb.jpg")),
                It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }

        [Fact(DisplayName = (nameof(ThrowExceptionRollbackUploadMediaErrorCase)))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task ThrowExceptionRollbackUploadMediaErrorCase()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var storageServiceMock = new Mock<IStorageService>();
            var repositoryCategoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMembersRepositoryMock = new Mock<ICastMemberRepository>();

            var input = _fixture.CreateValidInputAllMedias();
            var mediaNameToSave = $"media.{input.Media!.Extension}";
            var trailerNameToSave = $"trailer.{input.Media!.Extension}";
            var storageMediaPath = _fixture.GetValidMediaPath();
            var storageTrailerPath = _fixture.GetValidMediaPath();
            var storagePaths = new List<string>() { storageMediaPath, storageTrailerPath };


            storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(x => x.EndsWith(mediaNameToSave)),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(storageMediaPath);

            storageServiceMock.Setup(x => x.Upload(
               It.Is<string>(x => x.EndsWith(trailerNameToSave)),
               It.IsAny<Stream>(),
               It.IsAny<CancellationToken>()
               )).ReturnsAsync(storageTrailerPath);

            unitOfWork.Setup(x => x.Commit(
                It.IsAny<CancellationToken>()
                )).ThrowsAsync(new Exception("Something went wrony the commit"));




            var useCase = new UseCase.CreateVideo(
                repositoryMock.Object,
                repositoryCategoryMock.Object,
                genreRepositoryMock.Object,
                castMembersRepositoryMock.Object,
                unitOfWork.Object,
                storageServiceMock.Object
                    );

            var action = () => useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<Exception>()
                .WithMessage("Something went wrony the commit");

            storageServiceMock.Verify(x => x.Delete(
                It.Is<string>(x => storagePaths.Contains(x)),
                It.IsAny<CancellationToken>()),
                Times.Exactly(2));

            storageServiceMock.Verify(x => x.Delete(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }

    }
}
