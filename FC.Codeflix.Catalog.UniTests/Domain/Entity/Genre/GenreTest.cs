using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Xunit;
using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.UniTests.Domain.Entity.Genre
{
    [Collection(nameof(GenreTestFixture))]
    public class GenreTest
    {
        private readonly GenreTestFixture _fixture;
        public GenreTest(GenreTestFixture fixture)
        => _fixture = fixture;

        [Fact(DisplayName = nameof(Instatiate))]
        [Trait("Domain", "Genre - Aggregates")]
        public void Instatiate()
        {
            var datetimeBefore = DateTime.Now;
            var datetimeAfter = DateTime.Now.AddSeconds(1);
            var genre = _fixture.GetExampleGenre();

            genre.Id.Should().NotBeEmpty();
            genre.Name.Should().NotBeNull();
            genre.Name.Should().Be(genre.Name);
            genre.IsActive.Should().BeTrue();
            genre.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
            (genre.CreatedAt >= datetimeBefore).Should().BeTrue();
            (genre.CreatedAt <= datetimeAfter).Should().BeTrue();
        }

        [Theory(DisplayName = nameof(InstatiateWithIsActive))]
        [Trait("Domain", "Genre - Aggregates")]
        [InlineData(true)]
        [InlineData(false)]
        public void InstatiateWithIsActive(bool isActive)
        {
            var datetimeBefore = DateTime.Now;
            var datetimeAfter = DateTime.Now.AddSeconds(1);
            var genre = _fixture.GetExampleGenre(isActive);

            genre.Name.Should().NotBeNull();
            genre.Id.Should().NotBeEmpty();
            genre.Name.Should().Be(genre.Name);
            genre.IsActive.Should().Be(isActive);
            genre.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
            (genre.CreatedAt >= datetimeBefore).Should().BeTrue();
            (genre.CreatedAt <= datetimeAfter).Should().BeTrue();
        }

        [Theory(DisplayName = nameof(Activate))]
        [Trait("Domain", "Genre - Aggregates")]
        [InlineData(true)]
        [InlineData(false)]
        public void Activate(bool isActive)
        {
            var genre = _fixture.GetExampleGenre(isActive);

            genre.Activate();

            genre.Id.Should().NotBeEmpty();
            genre.Name.Should().NotBeNull();
            genre.IsActive.Should().BeTrue();
            genre.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        }

        [Theory(DisplayName = nameof(Deactivate))]
        [Trait("Domain", "Genre - Aggregates")]
        [InlineData(true)]
        [InlineData(false)]
        public void Deactivate(bool isActive)
        {
            var genre = _fixture.GetExampleGenre(isActive);

            genre.Deactivate();

            genre.Id.Should().NotBeEmpty();
            genre.Name.Should().NotBeNull();
            genre.IsActive.Should().BeFalse();
            genre.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        }

        [Fact(DisplayName = nameof(Update))]
        [Trait("Domain", "Genre - Aggregates")]
        public void Update()
        {
            var genre = _fixture.GetExampleGenre();
            var newName = _fixture.GetValidName();
            var oldIsActive = genre.IsActive;

            genre.Update(newName);

            genre.Id.Should().NotBeEmpty();
            genre.Name.Should().NotBeNull();
            genre.Name.Should().Be(newName);
            genre.IsActive.Should().Be(oldIsActive);
            genre.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        }

        [Theory(DisplayName = nameof(UpdateThrowWhenNameIsEmpty))]
        [Trait("Domain", "Genre - Aggregates")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void UpdateThrowWhenNameIsEmpty(string? name)
        {
            var genre = _fixture.GetExampleGenre();

            var action = () => genre.Update(name!);
            action.Should().Throw<EntityValidationException>()
           .WithMessage("Name should not be null or empty");
        }

        [Theory(DisplayName = nameof(InstatiateThrowWhenNameEmpty))]
        [Trait("Domain", "Genre - Aggregates")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void InstatiateThrowWhenNameEmpty(string? name)
        {
            var action = () => new DomainEntity.Genre(name!);

            action.Should().Throw<EntityValidationException>()
           .WithMessage("Name should not be null or empty");
        }

        [Fact(DisplayName = nameof(AddCategory))]
        [Trait("Domain", "Genre - Aggregates")]
        public void AddCategory()
        {
            var genre = _fixture.GetExampleGenre();
            var categoryGuid = Guid.NewGuid();
            genre.AddCategory(categoryGuid);

            genre.Categories.Should().HaveCount(1);
            genre.Categories.Should().Contain(categoryGuid);
        }

        [Fact(DisplayName = nameof(AddTwoCategory))]
        [Trait("Domain", "Genre - Aggregates")]
        public void AddTwoCategory()
        {
            var genre = _fixture.GetExampleGenre();
            var categoryGuid1 = Guid.NewGuid();
            var categoryGuid2 = Guid.NewGuid();

            genre.AddCategory(categoryGuid1);
            genre.AddCategory(categoryGuid2);

            genre.Categories.Should().HaveCount(2);
            genre.Categories.Should().Contain(categoryGuid1);
            genre.Categories.Should().Contain(categoryGuid2);
        }

        [Fact(DisplayName = nameof(RemoveAllCategory))]
        [Trait("Domain", "Genre - Aggregates")]
        public void RemoveAllCategory()
        {
            var exampleGuid = Guid.NewGuid();
            var genre = _fixture.GetExampleGenre(categoriesIdList: new List<Guid>()
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                exampleGuid,
                Guid.NewGuid(),
                Guid.NewGuid()
            }
                );

            genre.RemoveCategory(exampleGuid);

            genre.Categories.Should().HaveCount(4);
            genre.Categories.Should().NotContain(exampleGuid);

        }

        [Fact(DisplayName = nameof(RemoveCategory))]
        [Trait("Domain", "Genre - Aggregates")]
        public void RemoveCategory()
        {
            var genre = _fixture.GetExampleGenre(categoriesIdList: new List<Guid>()
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            }
                );

            genre.RemoveAllCategory();

            genre.Categories.Should().HaveCount(0);
        }
    }
}