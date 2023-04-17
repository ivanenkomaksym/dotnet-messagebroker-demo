namespace Common.Configuration
{
    public record RabbitMQOptions
    {
        public const string Name = "RabbitMQ";

        public bool UseStub { get; set; }
        public string HostName { get; set; }
    }
}
