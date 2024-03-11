using FC.Codeflix.Catalog.Domain.ValueObject;
using FC.Codeflix.Catalog.UniTests.Comon;
using FluentAssertions;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Domain.ValueObject
{
    public class ImageTest :BaseFixture
    {
        [Fact(DisplayName = nameof(Instatiate))]
        [Trait("Domain", "Image - ValueObjects")]
        public void Instatiate()
        {
            var path = Faker.Image.PicsumUrl();
            var image = new Image(path);
            image.Path.Should().Be(path);
        }

        [Fact(DisplayName = nameof(EqualsByPath))]
        [Trait("Domain", "Image - ValueObjects")]
        public void EqualsByPath()
        {
            var path = Faker.Image.PicsumUrl();
            var image = new Image(path);
            var sameImage = new Image(path);

            var isItEquals = image == sameImage;
            isItEquals.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(DifferrentByPath))]
        [Trait("Domain", "Image - ValueObjects")]
        public void DifferrentByPath()
        {
            var path = Faker.Image.PicsumUrl();
            var differentPath = Faker.Image.PicsumUrl();

            var image = new Image(path);
            var sameImage = new Image(differentPath);

            var isItDifferent = image != sameImage;
            isItDifferent.Should().BeTrue();
        }
    }
}
