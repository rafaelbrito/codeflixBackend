using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.UniTests.Domain.Entity.CastMember
{
    [Collection(nameof(CastMemberTestFixture))]
    public class CastMemberTest
    {
        private readonly CastMemberTestFixture _fixture;
        public CastMemberTest(CastMemberTestFixture fixture)
             => _fixture = fixture;

        [Fact(DisplayName = nameof(Instatiate))]
        [Trait("Domain", "CastMember - Aggregate")]
        public void Instatiate()
        {
            var dateTimeBefore = DateTime.Now.AddSeconds(-1);
            var name = _fixture.GetValidName();
            var type = _fixture.GetRandomCastMemberType();
            var castMember = new DomainEntity.CastMember(
                name, type
                );
            var dateTimeAfter = DateTime.Now.AddSeconds(1);
            castMember.Id.Should().NotBeEmpty();
            castMember.Name.Should().Be(name);
            castMember.Type.Should().Be(type);
            (castMember.CreatedAt >= dateTimeBefore).Should().BeTrue();
            (castMember.CreatedAt <= dateTimeAfter).Should().BeTrue();
        }

        [Theory(DisplayName = nameof(Instatiate))]
        [Trait("Domain", "CastMember - Aggregate")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void ThrowErrorWhenNameIsInvalid(string? name)
        {
            var type = _fixture.GetRandomCastMemberType();
            var action = () => new DomainEntity.CastMember(
                name!, type
                );
            action.Should().Throw<EntityValidationException>()
                .WithMessage($"Name should not be null or empty");
        }

        [Fact(DisplayName = nameof(Instatiate))]
        [Trait("Domain", "CastMember - Aggregate")]
        public void Update()
        {
            var name = _fixture.GetValidName();
            var type = _fixture.GetRandomCastMemberType();
            var castMember = _fixture.GetExampleCastMember();

            castMember.Update(name, type);

            castMember.Id.Should().NotBeEmpty();
            castMember.Name.Should().Be(name);
            castMember.Type.Should().Be(type);
        }

        [Theory(DisplayName = nameof(UpdateWithNameIsInvalid))]
        [Trait("Domain", "CastMember - Aggregate")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void UpdateWithNameIsInvalid(string? name)
        {
            var type = _fixture.GetRandomCastMemberType();
            var castMember = _fixture.GetExampleCastMember();

            var action = () => castMember.Update(name!, type);

            action.Should().Throw<EntityValidationException>()
                  .WithMessage($"Name should not be null or empty");
        }
    }
}
