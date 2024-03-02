using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.GetCastMember;
using FC.Codeflix.Catalog.Domain.Repository;
using Xunit;
using Moq;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.Exceptions;

namespace FC.Codeflix.Catalog.UniTests.Application.UseCases.CastMember.GetCastMember
{
    [Collection(nameof(GetCastMemberTestFixture))]
    public class GetCastMemberTest
    {
        private readonly GetCastMemberTestFixture _fixture;
        public GetCastMemberTest(GetCastMemberTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = (nameof(Get)))]
        [Trait("Application", "GetCastMember - Use Cases")]
        public async Task Get()
        {
            var repositoryMock = new Mock<ICastMemberRepository>();
            var exampleCastMember = _fixture.GetExampleCastMember();

            repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(exampleCastMember);

            var input = new UseCase.GetCastMemberInput(exampleCastMember.Id);
            var useCase = new UseCase.GetCastMember(repositoryMock.Object);
            var output = await useCase.Handle(input, CancellationToken.None);


            output.Id.Should().NotBeEmpty();
            output.Id.Should().Be(exampleCastMember.Id);
            output.Name.Should().Be(exampleCastMember.Name);
            output.Type.Should().Be(exampleCastMember.Type);

            repositoryMock.Verify(x => x.Get(It.Is<Guid>(x => x == input.Id),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = (nameof(NotFound)))]
        [Trait("Application", "GetCastMember - Use Cases")]
        public async Task NotFound()
        {
            var repositoryMock = new Mock<ICastMemberRepository>();

            repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Not Found"));

            var input = new UseCase.GetCastMemberInput(Guid.NewGuid());
            var useCase = new UseCase.GetCastMember(repositoryMock.Object);
            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>();
        }
    }
}
