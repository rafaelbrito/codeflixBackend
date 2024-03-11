using FC.Codeflix.Catalog.UniTests.Comon;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Xunit;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UniTests.Domain.Entity.Video
{
    [CollectionDefinition(nameof(VideoTestFixture))]
    public class VideoTestFixtureCollection : ICollectionFixture<VideoTestFixture>
    { }
    public class VideoTestFixture : BaseFixture
    {
        public DomainEntity.Video GetValidVideo()
            => new DomainEntity.Video(
                GetValidVideoTitle(),
                GetValidVideoDescription(),
                GetValidYearLauched(),
                GetRandomBoolean(),
                GetRandomBoolean(),
                GetValidVideoDuration(),
                GetRandomRating()
                );

        public string GetValidVideoTitle()
            => Faker.Name.FullName();

        public string GetValidVideoDescription()
            => Faker.Commerce.ProductDescription();

        public int GetValidYearLauched()
            => new Random().Next(1980, 2024);

        public int GetValidVideoDuration()
            => new Random().Next(5, 220);

        public List<DomainEntity.Video> GetExampleVideoList(int length = 10)
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

