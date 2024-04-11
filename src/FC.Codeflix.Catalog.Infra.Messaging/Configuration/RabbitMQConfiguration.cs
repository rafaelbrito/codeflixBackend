namespace FC.Codeflix.Catalog.Infra.Messaging.Configuration
{
    public class RabbitMQConfiguration
    {
        public const string CONFIGURATION_SECTION = "RabbitMQ";
        public string? HostName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Exchange { get; set; }
    }
}
