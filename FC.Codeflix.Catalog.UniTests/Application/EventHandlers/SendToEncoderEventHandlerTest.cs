using FC.Codeflix.Catalog.Application.EventHandlers;
using FC.Codeflix.Catalog.Application.Inferfaces;
using FC.Codeflix.Catalog.Domain.Events;
using Moq;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Application.EventHandlers
{
    public class SendToEncoderEventHandlerTest
    {
        [Fact(DisplayName = nameof(HandleAsync))]
        [Trait("Apllication", "EventHandlers")]
        public async Task HandleAsync()
        {
            var messageProducerMock = new Mock<IMessageProducer>();
            messageProducerMock.Setup(x => x.SendMessageAsync(
                It.IsAny<VideoUploadedEvent>(),
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var handler = new SendToEncoderEventHandler(messageProducerMock.Object);
            VideoUploadedEvent @event = new(Guid.NewGuid(), "medias/video.mp4");
            await handler.HandleAsync(@event, CancellationToken.None);

            messageProducerMock.Verify(x => x.SendMessageAsync(@event, CancellationToken.None), Times.Once());
        }
    }
}
