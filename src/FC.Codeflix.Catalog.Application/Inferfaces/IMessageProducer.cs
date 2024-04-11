namespace FC.Codeflix.Catalog.Application.Inferfaces
{
    public interface IMessageProducer
    {
        Task SendMessageAsync<T>(T message, CancellationToken cancellationToken);
    }
}
