using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Entity;
using Bogus;
using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using System.Text;

namespace FC.Codeflix.Catalog.UniTests.Common.Fixtures
{
    public class VideoBaseTestFixture : BaseFixture
    {
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

        public Video GetValidVideo()
            => new Video(
                GetValidVideoTitle(),
                GetValidVideoDescription(),
                GetValidYearLauched(),
                GetRandomBoolean(),
                GetRandomBoolean(),
                GetValidVideoDuration(),
                GetRandomRating()
                );

        public FileInput GetValidImageFileInput()
        {
            var exampleStream = new MemoryStream(Encoding.ASCII.GetBytes("test"));
            var fileInput = new FileInput("jpg", exampleStream);
            return fileInput;
        }

        public FileInput GetValidMediaFileInput()
        {
            var exampleStream = new MemoryStream(Encoding.ASCII.GetBytes("test"));
            var fileInput = new FileInput("mp4", exampleStream);
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
            GetValidVideo())
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
    }
}
