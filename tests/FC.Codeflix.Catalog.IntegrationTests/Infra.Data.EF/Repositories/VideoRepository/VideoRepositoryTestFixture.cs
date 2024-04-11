using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.IntegrationTests.Base;
using System.Text;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.VideoRepository
{
    [CollectionDefinition(nameof(VideoRepositoryTestFixture))]
    public class VideoRepositoryTestFixtureCollection : ICollectionFixture<VideoRepositoryTestFixture>
    { }
    public class VideoRepositoryTestFixture : BaseFixture
    {
        public bool GetRandomBoolean()
        => new Random().NextDouble() < 0.5;

        public List<Video> CloneVideoListOrdered(
        List<Video> videoList, string orderBy, SearchOrder order)
        {
            var listClone = new List<Video>(videoList);
            var orderedEnumerable = (orderBy.ToLower(), order) switch
            {
                ("title", SearchOrder.Asc) => listClone.OrderBy(x => x.Title)
                          .ThenBy(x => x.Id),
                ("title", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Title)
                         .ThenByDescending(x => x.Id),
                ("description", SearchOrder.Asc) => listClone.OrderBy(x => x.Description)
                         .ThenBy(x => x.Id),
                ("description", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Description)
                         .ThenByDescending(x => x.Id),
                ("id", SearchOrder.Asc) => listClone.OrderBy(x => x.Id),
                ("id", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
                ("createdat", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt),
                ("createdat", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt),
                _ => listClone.OrderBy(x => x.Title)
                        .ThenBy(x => x.Id),
            };
            return orderedEnumerable.ToList();
        }

        public List<Video> GetExampleVideoListByNames(List<string> titles)
           => titles.Select(title => GetValidVideo(title: title)).ToList();

        public CreateVideoInput GetValidVideoInput(
            List<Guid>? categoriesIds = null,
            List<Guid>? genresIds = null,
             List<Guid>? castMembersIds = null,
             FileInput? thumb = null,
             FileInput? banner = null,
             FileInput? thumbHalf = null,
             FileInput? media = null,
             FileInput? trailer = null)
           => new(
               GetValidVideoTitle(),
               GetValidVideoDescription(),
               GetValidYearLauched(),
               GetRandomBoolean(),
               GetRandomBoolean(),
               GetValidVideoDuration(),
               GetRandomRating(),
               categoriesIds,
               genresIds,
               castMembersIds,
               thumb,
               banner,
               thumbHalf,
               media,
               trailer
               );

        public CreateVideoInput GetValidVideoInputAllImages()
           => new(
               GetValidVideoTitle(),
               GetValidVideoDescription(),
               GetValidYearLauched(),
               GetRandomBoolean(),
               GetRandomBoolean(),
               GetValidVideoDuration(),
               GetRandomRating(),
               null,
               null,
               null,
               GetValidImageFileInput(),
               GetValidImageFileInput(),
               GetValidImageFileInput()
               );

        public Video GetValidVideo(string? title = null)
            => new Video(
                title ?? GetValidVideoTitle(),
                GetValidVideoDescription(),
                GetValidYearLauched(),
                GetRandomBoolean(),
                GetRandomBoolean(),
                GetValidVideoDuration(),
                GetRandomRating()
                );

        public Video GetValidVideoWithAllProperties()
        {
            var listCastMember = GetListRandomIds();
            var listGenres = GetListRandomIds();
            var listCategories = GetListRandomIds();
            var video = new Video(
                        GetValidVideoTitle(),
                        GetValidVideoDescription(),
                        GetValidYearLauched(),
                        GetRandomBoolean(),
                        GetRandomBoolean(),
                        GetValidVideoDuration(),
                        GetRandomRating()
                        );
            video.UpdateBanner(GetValidImagePath());
            video.UpdateThumb(GetValidImagePath());
            video.UpdateThumbHalf(GetValidImagePath());
            video.UpdateMedia(GetValidMediaPath());
            video.UpdateTrailer(GetValidMediaPath());
            listCastMember.ForEach(x => video.AddCastMember(x));
            listGenres.ForEach(x => video.AddGenre(x));
            listCategories.ForEach(x => video.AddCategory(x));

            return video;
        }

        public Video GetValidVideoWithAllMedias()
        {
            var video = new Video(
                        GetValidVideoTitle(),
                        GetValidVideoDescription(),
                        GetValidYearLauched(),
                        GetRandomBoolean(),
                        GetRandomBoolean(),
                        GetValidVideoDuration(),
                        GetRandomRating()
                        );
            video.UpdateBanner(GetValidImagePath());
            video.UpdateThumb(GetValidImagePath());
            video.UpdateThumbHalf(GetValidImagePath());
            video.UpdateMedia(GetValidMediaPath());
            video.UpdateTrailer(GetValidMediaPath());
            video.UpdateAsEncoded(GetValidImagePath());
            return video;
        }

        public FileInput GetValidImageFileInput()
        {
            var exampleStream = new MemoryStream(Encoding.ASCII.GetBytes("test"));
            var fileInput = new FileInput("jpg", exampleStream, "video/mp4");
            return fileInput;
        }

        public FileInput GetValidMediaFileInput()
        {
            var exampleStream = new MemoryStream(Encoding.ASCII.GetBytes("test"));
            var fileInput = new FileInput("mp4", exampleStream, "video/mp4");
            return fileInput;
        }

        public List<Guid> GetListRandomIds(int? count = null)
             => Enumerable.Range(1, count ?? (new Random().Next(1, 10)))
                .Select(_ => Guid.NewGuid())
                .ToList();

        public string GetValidVideoTitle()
            => Faker.Name.FullName();

        public string GetValidVideoDescription()
            => Faker.Commerce.ProductDescription();

        public int GetValidYearLauched()
            => new Random().Next(1980, 2024);

        public int GetValidVideoDuration()
            => new Random().Next(5, 220);

        public List<Video> GetExampleVideoList(int length = 10)
            => Enumerable.Range(0, length)
            .Select(_ =>
            GetValidVideoWithAllMedias())
            .ToList();

        public string GetTooLongTitle()
            => Faker.Lorem.Letter(400);

        public string GetTooLongDescription()
            => Faker.Lorem.Letter(4001);

        public Rating GetRandomRating()
        {
            var values = Enum.GetValues<Rating>();
            var random = new Random();
            return values[random.Next(values.Length)];
        }

        public string GetValidImagePath()
            => Faker.Image.PlaceImgUrl();

        public string GetValidMediaPath()
            => Faker.Internet.Url();

        public Media GetValidMedia()
            => new(GetValidMediaPath());

        public UpdateVideoInput CreateValidInput(
            Guid videoId, List<Guid>?
            genreIds = null,
            List<Guid>? categoriesIds = null,
            List<Guid>? castMembersIds = null,
            FileInput? thumb = null,
            FileInput? banner = null,
            FileInput? thumbHalf = null,
            FileInput? media = null,
            FileInput? trailer = null)
                => new UpdateVideoInput(
                   videoId,
                   GetValidVideoTitle(),
                   GetValidVideoDescription(),
                   GetValidYearLauched(),
                   GetRandomBoolean(),
                   GetRandomBoolean(),
                   GetValidVideoDuration(),
                   GetRandomRating(),
                   GenresIds: genreIds,
                   CategoriesIds: categoriesIds,
                   CastMembersIds: castMembersIds,
                   Thumb: thumb,
                   Banner: banner,
                   ThumbHalf: thumbHalf,
                   Media: media,
                   Trailer: trailer
                   );

        public string GetValidCastMemberName()
          => Faker.Name.FullName();

        public CastMemberType GetRandomCastMemberType()
            => (CastMemberType)(new Random().Next(1, 2));

        public CastMember GetExampleCastMember()
            => new(GetValidCastMemberName(), GetRandomCastMemberType());

        public List<CastMember> GetValidCastMemberList(int length = 10)
         => Enumerable.Range(0, length)
            .Select(_ =>
            GetExampleCastMember())
            .ToList();

        public string GetValidCategoryName()
        {
            var categoryName = "";
            while (categoryName.Length < 3)
                categoryName = Faker.Commerce.Categories(1)[0];
            if (categoryName.Length > 255)
                categoryName = categoryName[..255];
            return categoryName;
        }

        public string GetValidCategoryDescription()
        {
            var categoryDescription = Faker.Commerce.ProductDescription();
            if (categoryDescription.Length > 255)
                categoryDescription = categoryDescription[..10_000];
            return categoryDescription;
        }

        public Category GetExampleCategory()
         => new(GetValidCategoryName(),
                GetValidCategoryDescription(),
                GetRandomBoolean()
        );
        public List<Category> GetExampleCategoryList(int length = 10)
         => Enumerable.Range(0, length)
            .Select(_ =>
            GetExampleCategory())
            .ToList();

        public string GetValidGenreName()
           => Faker.Commerce.Categories(1)[0];

        public Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null, string? name = null)
        {
            var genre = new Genre(
                name ?? GetValidGenreName(),
                isActive ?? GetRandomBoolean()
                );
            categoriesIds?.ForEach(genre.AddCategory);
            return genre;
        }

        public List<Genre> GetExampleListGenres(int count = 10)
             => Enumerable
                .Range(1, count)
                .Select(_ => GetExampleGenre())
                .ToList();
    }
}
