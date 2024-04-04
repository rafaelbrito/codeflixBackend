using Xunit;
using FluentAssertions;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.VideoRepository
{
    [Collection(nameof(VideoRepositoryTestFixture))]
    public class VideoRepositoryTest
    {
        private readonly VideoRepositoryTestFixture _fixture;

        public VideoRepositoryTest(VideoRepositoryTestFixture fixture)
        => _fixture = fixture;

        [Fact(DisplayName = (nameof(Insert)))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task Insert()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleVideo = _fixture.GetValidVideo();
            var videoRepository = new Repository.VideoRepository(dbContext);

            await videoRepository.Insert(exampleVideo, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var assertsDbContext = _fixture.CreateDbContext(true);
            var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
            dbVideo.Should().NotBeNull();
            dbVideo!.Id.Should().Be(exampleVideo.Id);
            dbVideo.Title.Should().Be(exampleVideo.Title);
            dbVideo.Description.Should().Be(exampleVideo.Description);
            dbVideo.YearLauched.Should().Be(exampleVideo.YearLauched);
            dbVideo.Opened.Should().Be(exampleVideo.Opened);
            dbVideo.Published.Should().Be(exampleVideo.Published);
            dbVideo.Duration.Should().Be(exampleVideo.Duration);
            dbVideo.Rating.Should().Be(exampleVideo.Rating);
            dbVideo.CreatedAt.Should().NotBeSameDateAs(default);

            dbVideo.Thumb.Should().BeNull();
            dbVideo.ThumbHalf.Should().BeNull();
            dbVideo.Banner.Should().BeNull();
            dbVideo.Media.Should().BeNull();
            dbVideo.Trailer.Should().BeNull();

            dbVideo.Genres.Should().BeEmpty();
            dbVideo.Categories.Should().BeEmpty();
            dbVideo.CastMembers.Should().BeEmpty();
        }

        [Fact(DisplayName = (nameof(InsertWithMediasAndImages)))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task InsertWithMediasAndImages()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleVideo = _fixture.GetValidVideoWithAllMedias();
            var videoRepository = new Repository.VideoRepository(dbContext);

            await videoRepository.Insert(exampleVideo, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var assertsDbContext = _fixture.CreateDbContext(true);
            var dbVideo = await assertsDbContext.Videos
                .Include(x => x.Media)
                .Include(x => x.Trailer)
                .FirstOrDefaultAsync(video => video.Id == exampleVideo.Id);
            dbVideo.Should().NotBeNull();
            dbVideo!.Id.Should().Be(exampleVideo.Id);

            dbVideo.Thumb.Should().Be(exampleVideo.Thumb);
            dbVideo.Thumb!.Path.Should().Be(exampleVideo.ThumbHalf!.Path);
            dbVideo.ThumbHalf.Should().Be(exampleVideo.ThumbHalf);
            dbVideo.ThumbHalf!.Path.Should().Be(exampleVideo.ThumbHalf!.Path);
            dbVideo.Banner.Should().Be(exampleVideo.Banner);
            dbVideo.Banner!.Path.Should().Be(exampleVideo.Banner!.Path);

            dbVideo.Media.Should().NotBeNull();
            dbVideo.Media!.FilePath.Should().Be(exampleVideo.Media!.FilePath);
            dbVideo.Media!.EncodedPath.Should().Be(exampleVideo.Media.EncodedPath);
            dbVideo.Media!.Status.Should().Be(exampleVideo.Media!.Status);

            dbVideo.Trailer.Should().NotBeNull();
            dbVideo.Trailer!.FilePath.Should().Be(exampleVideo.Trailer!.FilePath);
            dbVideo.Trailer!.EncodedPath.Should().Be(exampleVideo.Trailer.EncodedPath);
            dbVideo.Trailer!.Status.Should().Be(exampleVideo.Trailer!.Status);

            dbVideo.Genres.Should().BeEmpty();
            dbVideo.Categories.Should().BeEmpty();
            dbVideo.CastMembers.Should().BeEmpty();
        }

        [Fact(DisplayName = (nameof(InsertWithRelations)))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task InsertWithRelations()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleVideo = _fixture.GetValidVideo();
            var castMembers = _fixture.GetValidCastMemberList();
            var categories = _fixture.GetExampleCategoryList();
            var genres = _fixture.GetExampleListGenres();

            genres.ToList().ForEach(genre
                => exampleVideo.AddGenre(genre.Id));

            categories.ToList().ForEach(category
                => exampleVideo.AddCategory(category.Id));

            castMembers.ToList().ForEach(castMember
                => exampleVideo.AddCastMember(castMember.Id));

            await dbContext.Genres.AddRangeAsync(genres);
            await dbContext.Categories.AddRangeAsync(categories);
            await dbContext.CastMembers.AddRangeAsync(castMembers);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var videoRepository = new Repository.VideoRepository(dbContext);

            await videoRepository.Insert(exampleVideo, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var assertsDbContext = _fixture.CreateDbContext(true);
            var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
            dbVideo.Should().NotBeNull();

            var videosCategories = assertsDbContext.VideosCategories
                .Where(relation => relation.VideoId == exampleVideo.Id);
            videosCategories.Should().HaveCount(categories.Count);
            videosCategories.Select(relation => relation.CategoryId)
                .ToList().Should()
                .BeEquivalentTo(categories.Select(category => category.Id));

            var videosGenres = assertsDbContext.VideosGenres
                .Where(relation => relation.VideoId == exampleVideo.Id);
            videosGenres.Should().HaveCount(genres.Count);
            videosGenres.Select(relation => relation.GenreId)
                .ToList().Should()
                .BeEquivalentTo(genres.Select(genre => genre.Id));

            var videosCastMembers = assertsDbContext.VideosCastMembers
                .Where(relation => relation.VideoId == exampleVideo.Id);
            videosCastMembers.Should().HaveCount(castMembers.Count);
            videosCastMembers.Select(relation => relation.CastMembersId)
                .ToList().Should()
                .BeEquivalentTo(castMembers.Select(castMember => castMember.Id));
        }

        [Fact(DisplayName = (nameof(Update)))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task Update()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleVideo = _fixture.GetValidVideo();
            await dbContext.AddAsync(exampleVideo);
            await dbContext.SaveChangesAsync();
            var newValueVideo = _fixture.GetValidVideo();
            var videoRepository = new Repository.VideoRepository(_fixture.CreateDbContext(true));
            exampleVideo.Update(
                newValueVideo.Title,
                newValueVideo.Description,
                newValueVideo.YearLauched,
                newValueVideo.Opened,
                newValueVideo.Published,
                newValueVideo.Duration,
                newValueVideo.Rating);

            await videoRepository.Update(exampleVideo, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var assertsDbContext = _fixture.CreateDbContext(true);
            var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
            dbVideo.Should().NotBeNull();
            dbVideo!.Id.Should().Be(exampleVideo.Id);
            dbVideo.Title.Should().Be(exampleVideo.Title);
            dbVideo.Description.Should().Be(exampleVideo.Description);
            dbVideo.YearLauched.Should().Be(exampleVideo.YearLauched);
            dbVideo.Opened.Should().Be(exampleVideo.Opened);
            dbVideo.Published.Should().Be(exampleVideo.Published);
            dbVideo.Duration.Should().Be(exampleVideo.Duration);
            dbVideo.Rating.Should().Be(exampleVideo.Rating);
            dbVideo.CreatedAt.Should().NotBeSameDateAs(default);

            dbVideo.Thumb.Should().BeNull();
            dbVideo.ThumbHalf.Should().BeNull();
            dbVideo.Banner.Should().BeNull();
            dbVideo.Media.Should().BeNull();
            dbVideo.Trailer.Should().BeNull();

            dbVideo.Genres.Should().BeEmpty();
            dbVideo.Categories.Should().BeEmpty();
            dbVideo.CastMembers.Should().BeEmpty();
        }

        [Fact(DisplayName = (nameof(UpdateEntitiesWhenValueObjects)))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task UpdateEntitiesWhenValueObjects()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleVideo = _fixture.GetValidVideo();
            await dbContext.AddAsync(exampleVideo);
            await dbContext.SaveChangesAsync();

            var updateThumb = _fixture.GetValidImagePath();
            var updateThumbHalf = _fixture.GetValidImagePath();
            var updateBanner = _fixture.GetValidImagePath();
            var updateMedia = _fixture.GetValidMediaPath();
            var updateTrailer = _fixture.GetValidMediaPath();
            var updateMediaEncoded = _fixture.GetValidMediaPath();
            var dbContextAct = _fixture.CreateDbContext(true);
            var videoRepository = new Repository.VideoRepository(dbContextAct);
            var savedVideo = dbContextAct.Videos.Single(video => video.Id == exampleVideo.Id);

            savedVideo.UpdateThumb(updateThumb);
            savedVideo.UpdateThumbHalf(updateThumbHalf);
            savedVideo.UpdateBanner(updateBanner);
            savedVideo.UpdateMedia(updateMedia);
            savedVideo.UpdateTrailer(updateTrailer);
            savedVideo.UpdateAsEncoded(updateMediaEncoded);

            await videoRepository.Update(savedVideo, CancellationToken.None);
            await dbContextAct.SaveChangesAsync(CancellationToken.None);

            var assertsDbContext = _fixture.CreateDbContext(true);
            var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
            dbVideo.Should().NotBeNull();
            dbVideo!.Media.Should().NotBeNull();
            dbVideo.Trailer.Should().NotBeNull();
            dbVideo.Thumb.Should().NotBeNull();
            dbVideo.ThumbHalf.Should().NotBeNull();
            dbVideo.Banner.Should().NotBeNull();
            dbVideo.Thumb!.Path.Should().Be(updateThumb);
            dbVideo.ThumbHalf!.Path.Should().Be(updateThumbHalf);
            dbVideo.Banner!.Path!.Should().Be(updateBanner);
            dbVideo.Media!.FilePath.Should().Be(updateMedia);
            dbVideo.Media!.EncodedPath.Should().Be(updateMediaEncoded);
            dbVideo.Media.Status.Should().Be(MediaStatus.Completed);
            dbVideo.Trailer!.FilePath.Should().Be(updateTrailer);
        }

        [Fact(DisplayName = (nameof(UpdatetWithRelations)))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task UpdatetWithRelations()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleVideo = _fixture.GetValidVideo();
            var castMembers = _fixture.GetValidCastMemberList();
            var categories = _fixture.GetExampleCategoryList();
            var genres = _fixture.GetExampleListGenres();
            await dbContext.Videos.AddAsync(exampleVideo);

            await dbContext.Genres.AddRangeAsync(genres);
            await dbContext.Categories.AddRangeAsync(categories);
            await dbContext.CastMembers.AddRangeAsync(castMembers);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var dbContextAct = _fixture.CreateDbContext(true);
            var savedVideo = dbContextAct.Videos.Single(video => video.Id == exampleVideo.Id);

            genres.ToList().ForEach(genre
               => savedVideo.AddGenre(genre.Id));

            categories.ToList().ForEach(category
                => savedVideo.AddCategory(category.Id));

            castMembers.ToList().ForEach(castMember
                => savedVideo.AddCastMember(castMember.Id));


            var videoRepository = new Repository.VideoRepository(dbContextAct);

            await videoRepository.Update(savedVideo, CancellationToken.None);
            await dbContextAct.SaveChangesAsync(CancellationToken.None);

            var assertsDbContext = _fixture.CreateDbContext(true);
            var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
            dbVideo.Should().NotBeNull();

            var videosCategories = assertsDbContext.VideosCategories
                .Where(relation => relation.VideoId == exampleVideo.Id);
            videosCategories.Should().HaveCount(categories.Count);
            videosCategories.Select(relation => relation.CategoryId)
                .ToList().Should()
                .BeEquivalentTo(categories.Select(category => category.Id));

            var videosGenres = assertsDbContext.VideosGenres
                .Where(relation => relation.VideoId == exampleVideo.Id);
            videosGenres.Should().HaveCount(genres.Count);
            videosGenres.Select(relation => relation.GenreId)
                .ToList().Should()
                .BeEquivalentTo(genres.Select(genre => genre.Id));

            var videosCastMembers = assertsDbContext.VideosCastMembers
                .Where(relation => relation.VideoId == exampleVideo.Id);
            videosCastMembers.Should().HaveCount(castMembers.Count);
            videosCastMembers.Select(relation => relation.CastMembersId)
                .ToList().Should()
                .BeEquivalentTo(castMembers.Select(castMember => castMember.Id));
        }

        [Fact(DisplayName = (nameof(Delete)))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task Delete()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleVideo = _fixture.GetValidVideoWithAllMedias();
            await dbContext.AddAsync(exampleVideo);
            await dbContext.SaveChangesAsync();
            var dbContextAct = _fixture.CreateDbContext(true);
            var savedVideo = dbContextAct.Videos.Single(video => video.Id == exampleVideo.Id);
            var videoRepository = new Repository.VideoRepository(dbContextAct);

            await videoRepository.Delete(savedVideo, CancellationToken.None);
            await dbContextAct.SaveChangesAsync(CancellationToken.None);

            var assertsDbContext = _fixture.CreateDbContext(true);
            var dbVideo = await assertsDbContext.Videos.FindAsync(savedVideo.Id);
            dbVideo.Should().BeNull();
        }

        [Fact(DisplayName = (nameof(DeleteWithRelationsAndMedia)))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task DeleteWithRelationsAndMedia()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleVideo = _fixture.GetValidVideoWithAllMedias();
            var castMembers = _fixture.GetValidCastMemberList();
            var categories = _fixture.GetExampleCategoryList();
            var genres = _fixture.GetExampleListGenres();

            genres.ToList().ForEach(genre
                => dbContext.VideosGenres.Add(new(genre.Id, exampleVideo.Id)));

            categories.ToList().ForEach(category
                => dbContext.VideosCategories.Add(new(category.Id, exampleVideo.Id)));

            castMembers.ToList().ForEach(castMember
                => dbContext.VideosCastMembers.Add(new(castMember.Id, exampleVideo.Id)));

            await dbContext.Genres.AddRangeAsync(genres);
            await dbContext.Categories.AddRangeAsync(categories);
            await dbContext.CastMembers.AddRangeAsync(castMembers);
            await dbContext.Videos.AddRangeAsync(exampleVideo);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var dbContextAct = _fixture.CreateDbContext(true);
            var videoRepository = new Repository.VideoRepository(dbContextAct);
            var savedVideo = dbContextAct.Videos.First(video => video.Id == exampleVideo.Id);

            await videoRepository.Delete(savedVideo, CancellationToken.None);
            await dbContextAct.SaveChangesAsync(CancellationToken.None);

            var assertsDbContext = _fixture.CreateDbContext(true);
            var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
            dbVideo.Should().BeNull();

            assertsDbContext.VideosCategories
               .Where(relation => relation.VideoId == exampleVideo.Id)
               .ToList().Count().Should().Be(0);


            var videosGenres = assertsDbContext.VideosGenres
                .Where(relation => relation.VideoId == exampleVideo.Id)
                .ToList().Count().Should().Be(0);

            var videosCastMembers = assertsDbContext.VideosCastMembers
                .Where(relation => relation.VideoId == exampleVideo.Id)
                .ToList().Count().Should().Be(0);
            assertsDbContext.Set<Media>().Count().Should().Be(0);
        }

        [Fact(DisplayName = (nameof(Get)))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task Get()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleVideo = _fixture.GetValidVideoWithAllMedias();
            await dbContext.AddAsync(exampleVideo);
            await dbContext.SaveChangesAsync();
            var dbContextAct = _fixture.CreateDbContext(true);
            var videoRepository = new Repository.VideoRepository(dbContextAct);

            var video = await videoRepository.Get(exampleVideo.Id, CancellationToken.None);

            video.Should().NotBeNull();
            video!.Id.Should().Be(exampleVideo.Id);
            video.Title.Should().Be(exampleVideo.Title);
            video.Description.Should().Be(exampleVideo.Description);
            video.YearLauched.Should().Be(exampleVideo.YearLauched);
            video.Opened.Should().Be(exampleVideo.Opened);
            video.Published.Should().Be(exampleVideo.Published);
            video.Duration.Should().Be(exampleVideo.Duration);
            video.Rating.Should().Be(exampleVideo.Rating);
        }

        [Fact(DisplayName = (nameof(Search)))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task Search()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleVideoList = _fixture.GetExampleVideoList();
            await dbContext.AddRangeAsync(exampleVideoList);
            await dbContext.SaveChangesAsync();
            var dbContextAct = _fixture.CreateDbContext(true);
            var videoRepository = new Repository.VideoRepository(dbContextAct);
            var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

            var searchResult = await videoRepository.Search(searchInput, CancellationToken.None);

            searchResult.Should().NotBeNull();
            searchResult.CurrentPage.Should().Be(searchInput.Page);
            searchResult.PerPage.Should().Be(searchInput.PerPage);
            searchResult.Total.Should().Be(exampleVideoList.Count);
            searchResult.Items.Should().NotBeNull();
            searchResult.Items.Should().HaveCount(exampleVideoList.Count);

            foreach (var resultItem in searchResult.Items)
            {
                var exampleVideo = exampleVideoList
                    .Find(x => x.Id == resultItem.Id);
                exampleVideo.Should().NotBeNull();
                resultItem.Should().NotBeNull();
                resultItem!.Id.Should().Be(exampleVideo!.Id);
                resultItem.Title.Should().Be(exampleVideo.Title);
                resultItem.Description.Should().Be(exampleVideo.Description);
                resultItem.YearLauched.Should().Be(exampleVideo.YearLauched);
                resultItem.Opened.Should().Be(exampleVideo.Opened);
                resultItem.Published.Should().Be(exampleVideo.Published);
                resultItem.Duration.Should().Be(exampleVideo.Duration);
                resultItem.Rating.Should().Be(exampleVideo.Rating);

                resultItem.Thumb!.Path.Should().Be(exampleVideo.Thumb!.Path);
                resultItem.ThumbHalf!.Path.Should().Be(exampleVideo.ThumbHalf!.Path);
                resultItem.Banner!.Path.Should().Be(exampleVideo.Banner!.Path);
                resultItem.Media!.FilePath.Should().Be(exampleVideo.Media!.FilePath);
                resultItem.Trailer!.FilePath.Should().Be(exampleVideo.Trailer!.FilePath);

                resultItem.Genres.Should().BeEmpty();
                resultItem.Categories.Should().BeEmpty();
                resultItem.CastMembers.Should().BeEmpty();

            }
        }

        [Fact(DisplayName = (nameof(SearchWithRelations)))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task SearchWithRelations()
        {
            var exampleVideoList = _fixture.GetExampleVideoList();
        
            using (var dbContext = _fixture.CreateDbContext())
            {
                foreach (var exampleVideo in exampleVideoList)
                {
                    var castMembers = _fixture.GetValidCastMemberList();
                    var categories = _fixture.GetExampleCategoryList();
                    var genres = _fixture.GetExampleListGenres();
                    genres.ToList().ForEach(genre =>
                    {
                        exampleVideo.AddGenre(genre.Id);
                        dbContext.VideosGenres.Add(new(exampleVideo.Id, genre.Id));
                    });

                    categories.ToList().ForEach(category =>
                    {
                        exampleVideo.AddCategory(category.Id);
                        dbContext.VideosCategories.Add(new(exampleVideo.Id, category.Id));
                    });

                    castMembers.ToList().ForEach(castMember =>
                    {
                        exampleVideo.AddCastMember(castMember.Id);
                        dbContext.VideosCastMembers.Add(new(exampleVideo.Id, castMember.Id));
                    });
                    await dbContext.Categories.AddRangeAsync(categories);
                    await dbContext.Genres.AddRangeAsync(genres);
                    await dbContext.CastMembers.AddRangeAsync(castMembers);
                }
                await dbContext.Videos.AddRangeAsync(exampleVideoList);
                await dbContext.SaveChangesAsync(CancellationToken.None);
            }

            var dbContextAct = _fixture.CreateDbContext(true);
            var videoRepository = new Repository.VideoRepository(dbContextAct);
            var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

            var searchResult = await videoRepository.Search(searchInput, CancellationToken.None);

            searchResult.Should().NotBeNull();
            searchResult.CurrentPage.Should().Be(searchInput.Page);
            searchResult.PerPage.Should().Be(searchInput.PerPage);
            searchResult.Total.Should().Be(exampleVideoList.Count);
            searchResult.Items.Should().NotBeNull();
            searchResult.Items.Should().HaveCount(exampleVideoList.Count);
            searchResult.Items.ToList().ForEach(resultItem =>
            {
                var exampleVideo = exampleVideoList.FirstOrDefault(x => x.Id == resultItem.Id);
                exampleVideo.Should().NotBeNull();
                resultItem.Genres.Should().BeEquivalentTo(exampleVideo!.Genres);
                resultItem.Categories.Should().BeEquivalentTo(exampleVideo!.Categories);
                resultItem.CastMembers.Should().BeEquivalentTo(exampleVideo!.CastMembers);
            }); 
        }

        [Theory(DisplayName = (nameof(SearchReturnsPaginated)))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
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
            var dbContext = _fixture.CreateDbContext();
            var exampleVideoList = _fixture.GetExampleVideoList(quantityToGenerate);
            await dbContext.AddRangeAsync(exampleVideoList);
            await dbContext.SaveChangesAsync();
            var dbContextAct = _fixture.CreateDbContext(true);
            var videoRepository = new Repository.VideoRepository(dbContextAct);
            var searchInput = new SearchInput(page, perPage, "", "", SearchOrder.Asc);

            var searchResult = await videoRepository.Search(searchInput, CancellationToken.None);

            searchResult.Should().NotBeNull();
            searchResult.CurrentPage.Should().Be(searchInput.Page);
            searchResult.PerPage.Should().Be(searchInput.PerPage);
            searchResult.Total.Should().Be(exampleVideoList.Count);
            searchResult.Items.Should().NotBeNull();
            searchResult.Items.Should().HaveCount(expectedQuantityItems);

            foreach (var resultItem in searchResult.Items)
            {
                var exampleVideo = exampleVideoList
                    .Find(x => x.Id == resultItem.Id);
                exampleVideo.Should().NotBeNull();
                resultItem.Should().NotBeNull();
                resultItem!.Id.Should().Be(exampleVideo!.Id);
                resultItem.Title.Should().Be(exampleVideo.Title);
                resultItem.Description.Should().Be(exampleVideo.Description);
                resultItem.YearLauched.Should().Be(exampleVideo.YearLauched);
                resultItem.Opened.Should().Be(exampleVideo.Opened);
                resultItem.Published.Should().Be(exampleVideo.Published);
                resultItem.Duration.Should().Be(exampleVideo.Duration);
                resultItem.Rating.Should().Be(exampleVideo.Rating);

                resultItem.Thumb!.Path.Should().Be(exampleVideo.Thumb!.Path);
                resultItem.ThumbHalf!.Path.Should().Be(exampleVideo.ThumbHalf!.Path);
                resultItem.Banner!.Path.Should().Be(exampleVideo.Banner!.Path);
                resultItem.Media!.FilePath.Should().Be(exampleVideo.Media!.FilePath);
                resultItem.Trailer!.FilePath.Should().Be(exampleVideo.Trailer!.FilePath);

                resultItem.Genres.Should().BeEmpty();
                resultItem.Categories.Should().BeEmpty();
                resultItem.CastMembers.Should().BeEmpty();
            }
        }

        [Theory(DisplayName = (nameof(SearchByText)))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
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
            var exampleVideoList = _fixture
                .GetExampleVideoListByNames(new List<string>() {
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


            await dbContext.AddRangeAsync(exampleVideoList);
            await dbContext.SaveChangesAsync();
            var dbContextAct = _fixture.CreateDbContext(true);
            var videoRepository = new Repository.VideoRepository(dbContextAct);
            var searchInput = new SearchInput(page, perPage, search, "", SearchOrder.Asc);

            var searchResult = await videoRepository.Search(searchInput, CancellationToken.None);

            searchResult.Should().NotBeNull();
            searchResult.CurrentPage.Should().Be(searchInput.Page);
            searchResult.PerPage.Should().Be(searchInput.PerPage);
            searchResult.Total.Should().Be(expectedQuantityTotalItems);
            searchResult.Items.Should().NotBeNull();
            searchResult.Items.Should().HaveCount(expectedQuantityItemsReturned);

            foreach (var resultItem in searchResult.Items)
            {
                var exampleVideo = exampleVideoList
                    .Find(x => x.Id == resultItem.Id);
                exampleVideo.Should().NotBeNull();
                resultItem.Should().NotBeNull();
                resultItem!.Id.Should().Be(exampleVideo!.Id);
                resultItem.Title.Should().Be(exampleVideo.Title);
                resultItem.Description.Should().Be(exampleVideo.Description);
                resultItem.YearLauched.Should().Be(exampleVideo.YearLauched);
                resultItem.Opened.Should().Be(exampleVideo.Opened);
                resultItem.Published.Should().Be(exampleVideo.Published);
                resultItem.Duration.Should().Be(exampleVideo.Duration);
                resultItem.Rating.Should().Be(exampleVideo.Rating);

                resultItem.Genres.Should().BeEmpty();
                resultItem.Categories.Should().BeEmpty();
                resultItem.CastMembers.Should().BeEmpty();
            }
        }

        [Theory(DisplayName = (nameof(SearchReturnsOrdered)))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        [InlineData("title", "asc")]
        [InlineData("title", "desc")]
        [InlineData("description", "asc")]
        [InlineData("description", "desc")]
        [InlineData("id", "asc")]
        [InlineData("id", "desc")]
        [InlineData("createdAt", "asc")]
        [InlineData("createdAt", "desc")]
        [InlineData("", "asc")]
        public async Task SearchReturnsOrdered(string orderBy, string order)
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleVideoList = _fixture.GetExampleVideoList();
            await dbContext.AddRangeAsync(exampleVideoList);
            await dbContext.SaveChangesAsync();
            var dbContextAct = _fixture.CreateDbContext(true);
            var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
            var searchInput = new SearchInput(1, 20, "", orderBy.ToLower(), searchOrder);

            var videoRepository = new Repository.VideoRepository(dbContextAct);

            var searchResult = await videoRepository.Search(searchInput, CancellationToken.None);

            var expectedOrderedList = _fixture.CloneVideoListOrdered(exampleVideoList, orderBy.ToLower(), searchOrder);


            searchResult.Should().NotBeNull();
            searchResult.CurrentPage.Should().Be(searchInput.Page);
            searchResult.PerPage.Should().Be(searchInput.PerPage);
            searchResult.Total.Should().Be(exampleVideoList.Count);
            searchResult.Items.Should().NotBeNull();
            searchResult.Items.Should().HaveCount(exampleVideoList.Count);

            foreach (var resultItem in searchResult.Items)
            {
                var exampleVideo = exampleVideoList
                    .Find(x => x.Id == resultItem.Id);
                exampleVideo.Should().NotBeNull();
                resultItem.Should().NotBeNull();
                resultItem!.Id.Should().Be(exampleVideo!.Id);
                resultItem.Title.Should().Be(exampleVideo.Title);
                resultItem.Description.Should().Be(exampleVideo.Description);
                resultItem.YearLauched.Should().Be(exampleVideo.YearLauched);
                resultItem.Opened.Should().Be(exampleVideo.Opened);
                resultItem.Published.Should().Be(exampleVideo.Published);
                resultItem.Duration.Should().Be(exampleVideo.Duration);
                resultItem.Rating.Should().Be(exampleVideo.Rating);

                resultItem.Genres.Should().BeEmpty();
                resultItem.Categories.Should().BeEmpty();
                resultItem.CastMembers.Should().BeEmpty();
            }
        }

        [Fact(DisplayName = (nameof(SearchReturnsEmpty)))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task SearchReturnsEmpty()
        {
            var dbContext = _fixture.CreateDbContext();
            var videoRepository = new Repository.VideoRepository(dbContext);
            var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

            var searchResult = await videoRepository.Search(searchInput, CancellationToken.None);

            searchResult.Should().NotBeNull();
            searchResult.CurrentPage.Should().Be(searchInput.Page);
            searchResult.PerPage.Should().Be(searchInput.PerPage);
            searchResult.Total.Should().Be(0);
            searchResult.Items.Should().NotBeNull();
            searchResult.Items.Should().HaveCount(0);
        }

        [Fact(DisplayName = (nameof(GetWithRelations)))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task GetWithRelations()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleVideo = _fixture.GetValidVideoWithAllMedias();
            var castMembers = _fixture.GetValidCastMemberList();
            var categories = _fixture.GetExampleCategoryList();
            var genres = _fixture.GetExampleListGenres();

            genres.ToList().ForEach(genre
                =>
            {
                exampleVideo.AddGenre(genre.Id);
                dbContext.VideosGenres.Add(new(exampleVideo.Id, genre.Id));
            });

            categories.ToList().ForEach(category
                =>
            {
                exampleVideo.AddCategory(category.Id);
                dbContext.VideosCategories.Add(new(exampleVideo.Id, category.Id));
            });

            castMembers.ToList().ForEach(castMember
                =>
            {
                exampleVideo.AddCastMember(castMember.Id);
                dbContext.VideosCastMembers.Add(new(exampleVideo.Id, castMember.Id));
            });

            await dbContext.Genres.AddRangeAsync(genres);
            await dbContext.Categories.AddRangeAsync(categories);
            await dbContext.CastMembers.AddRangeAsync(castMembers);
            await dbContext.Videos.AddRangeAsync(exampleVideo);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var videoRepository = new Repository.VideoRepository(dbContext);

            var video = await videoRepository.Get(exampleVideo.Id, CancellationToken.None);

            video.Should().NotBeNull();
            video!.Id.Should().Be(exampleVideo.Id);
            video.Title.Should().Be(exampleVideo.Title);
            video.Description.Should().Be(exampleVideo.Description);
            video.YearLauched.Should().Be(exampleVideo.YearLauched);
            video.Opened.Should().Be(exampleVideo.Opened);
            video.Published.Should().Be(exampleVideo.Published);
            video.Duration.Should().Be(exampleVideo.Duration);
            video.Rating.Should().Be(exampleVideo.Rating);

            video!.Media.Should().NotBeNull();
            video.Trailer.Should().NotBeNull();
            video.Thumb.Should().NotBeNull();
            video.ThumbHalf.Should().NotBeNull();
            video.Banner.Should().NotBeNull();
            video.Thumb!.Path.Should().Be(exampleVideo.Thumb!.Path);
            video.ThumbHalf!.Path.Should().Be(exampleVideo.ThumbHalf!.Path);
            video.Banner!.Path!.Should().Be(exampleVideo.Banner!.Path);
            video.Media!.FilePath.Should().Be(exampleVideo.Media!.FilePath);
            video.Media!.EncodedPath.Should().Be(exampleVideo.Media.EncodedPath);
            video.Media.Status.Should().Be(MediaStatus.Completed);
            video.Trailer!.FilePath.Should().Be(exampleVideo.Trailer!.FilePath);

            video.Genres.Should().BeEquivalentTo(exampleVideo.Genres);
            video.Categories.Should().BeEquivalentTo(exampleVideo.Categories);
            video.CastMembers.Should().BeEquivalentTo(exampleVideo.CastMembers);

        }

        [Fact(DisplayName = (nameof(ThrowsVideoNotFound)))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task ThrowsVideoNotFound()
        {
            var videoRepository = new Repository.VideoRepository(_fixture.CreateDbContext());
            var id = Guid.NewGuid();
            var action = () => videoRepository.Get(id, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>()
                 .WithMessage($"Video '{id}' not found.");
        }
    }
}
