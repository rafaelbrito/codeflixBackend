using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.DeleteCastMember;
using FC.Codeflix.Catalog.Domain.Repository;
using Xunit;
using Moq;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Inferfaces;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UniTests.Application.UseCases.CastMember.DeleteCastMember
{
    [Collection(nameof(DeleteCastMemberTestFixture))]
    public class DeleteCastMemberTest
    {
        private readonly DeleteCastMemberTestFixture _fixture;
        public DeleteCastMemberTest(DeleteCastMemberTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = (nameof(Delete)))]
        [Trait("Application", "DeleteCastmember - Use Cases")]
        public async Task Delete()
        {
            var repositoryMock = new Mock<ICastMemberRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var exampleCastMember = _fixture.GetExampleCastMember();

            repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(exampleCastMember);

            var input = new UseCase.DeleteCastMemberInput(exampleCastMember.Id);
            var useCase = new UseCase.DeleteCastMember(repositoryMock.Object, unitOfWorkMock.Object);
            var action = async () =>await useCase.Handle(input, CancellationToken.None);
            await action.Should().NotThrowAsync();

            repositoryMock.Verify(x => x.Get(It.Is<Guid>(x => x == input.Id),
                It.IsAny<CancellationToken>()), Times.Once);

            repositoryMock.Verify(x => x.Delete(
                It.Is<DomainEntity.CastMember>(x => x.Id == input.Id),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = (nameof(ThrowWhenNotFound)))]
        [Trait("Application", "DeleteCastmember - Use Cases")]
        public async Task ThrowWhenNotFound()
        {
            var repositoryMock = new Mock<ICastMemberRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Not Found"));

            var input = new UseCase.DeleteCastMemberInput(Guid.NewGuid());
            var useCase = new UseCase.DeleteCastMember(repositoryMock.Object, unitOfWorkMock.Object);
            var action = async () => await useCase.Handle(input, CancellationToken.None);
            await action.Should().ThrowAsync<NotFoundException>();
        }
    }
}
