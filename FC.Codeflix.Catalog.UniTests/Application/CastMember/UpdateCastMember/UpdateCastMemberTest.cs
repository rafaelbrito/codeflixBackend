using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using FC.Codeflix.Catalog.Domain.Repository;
using Xunit;
using Moq;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.Inferfaces;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Application.Exceptions;
using System.Runtime.CompilerServices;
using FC.Codeflix.Catalog.Domain.Exceptions;
namespace FC.Codeflix.Catalog.UniTests.Application.UseCases.CastMember.UpdateCastMember
{
    [Collection(nameof(UpdateCastMemberTestFixture))]
    public class UpdateCastMemberTest
    {
        private readonly UpdateCastMemberTestFixture _fixture;
        public UpdateCastMemberTest(UpdateCastMemberTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = (nameof(Update)))]
        [Trait("Application", "UpdateCastMember - Use Cases")]
        public async Task Update()
        {
            var repositoryMock = new Mock<ICastMemberRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var newName = _fixture.GetValidName();
            var newType = _fixture.GetRandomCastMemberType();
            var exampleCastMember = _fixture.GetExampleCastMember();

            repositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == exampleCastMember.Id),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(exampleCastMember);

            var input = new UseCase.UpdateCastMemberInput(
                exampleCastMember.Id,
                newName,
                newType
                );

            var useCase = new UseCase.UpdateCastMember(repositoryMock.Object, unitOfWorkMock.Object);
            var output = await useCase.Handle(input, CancellationToken.None);


            output.Id.Should().Be(exampleCastMember.Id);
            output.Name.Should().Be(newName);
            output.Type.Should().Be(newType);

            repositoryMock.Verify(x => x.Get(It.Is<Guid>(x => x == input.Id),
                It.IsAny<CancellationToken>()), Times.Once);

            repositoryMock.Verify(x => x.Update(It.Is<DomainEntity.CastMember>(
                x => x.Id == input.Id
                && x.Name == input.Name
                && x.Type == input.Type),
                It.IsAny<CancellationToken>()), Times.Once);

            unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = (nameof(ThrowWhenNotFound)))]
        [Trait("Application", "UpdateCastMember - Use Cases")]
        public async Task ThrowWhenNotFound()
        {
            var repositoryMock = new Mock<ICastMemberRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Not Found"));

            var input = new UseCase.UpdateCastMemberInput(
                Guid.NewGuid(),
                _fixture.GetValidName(),
                _fixture.GetRandomCastMemberType()
                );

            var useCase = new UseCase.UpdateCastMember(repositoryMock.Object, unitOfWorkMock.Object);
            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>();
        }

        [Theory(DisplayName = (nameof(ThrowWhenNotFound)))]
        [Trait("Application", "UpdateCastMember - Use Cases")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task ThrowWhenInValidName(string? name)
        {
            var repositoryMock = new Mock<ICastMemberRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var exampleCastMember = _fixture.GetExampleCastMember();

            repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(exampleCastMember);

            var input = new UseCase.UpdateCastMemberInput(
                Guid.NewGuid(),
                name!,
                _fixture.GetRandomCastMemberType()
                );

            var useCase = new UseCase.UpdateCastMember(repositoryMock.Object, unitOfWorkMock.Object);
            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<EntityValidationException>()
                .WithMessage("Name should not be empty or null");
        }
    }
}
