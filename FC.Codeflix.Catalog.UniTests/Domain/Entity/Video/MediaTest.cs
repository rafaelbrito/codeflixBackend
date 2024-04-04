using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FluentAssertions;
using Xunit;


namespace FC.Codeflix.Catalog.UniTests.Domain.Entity.Video
{
    [Collection(nameof(VideoTestFixture))]
    public class MediaTest
    {
        private readonly VideoTestFixture _fixture;
        public MediaTest(VideoTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(Instantiate))]
        [Trait("Domain", "Media - Entities")]
        public void Instantiate()
        {
            var path = _fixture.GetValidMediaPath();
            var media = new Media(path);
            media.Id.Should().NotBeEmpty(default);
            media.FilePath.Should().Be(path);
            media.Status.Should().Be(MediaStatus.Pending);
        }

        [Fact(DisplayName = nameof(UpdateSentToEncode))]
        [Trait("Domain", "Media - Entities")]
        public void UpdateSentToEncode()
        {
            var media = _fixture.GetValidMedia();

            media.UpdateAsSentToEncode();
            media.Status.Should().Be(MediaStatus.Processing);
        }

        [Fact(DisplayName = nameof(UpdateAsEncode))]
        [Trait("Domain", "Media - Entities")]
        public void UpdateAsEncode()
        {
            var media = _fixture.GetValidMedia();
            var examplePath = _fixture.GetValidMediaPath();
            media.UpdateAsSentToEncode();
            media.UpdateAsEncoded(examplePath);
            media.Status.Should().Be(MediaStatus.Completed);
            media.EncodedPath.Should().Be(examplePath);
        }
    }
}
