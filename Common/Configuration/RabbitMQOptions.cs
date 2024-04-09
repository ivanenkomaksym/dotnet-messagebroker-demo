namespace Common.Configuration
{
    public record RabbitMQOptions
    {
        public const string Name = "RabbitMQ";

        public required string AMQPConnectionString { get; set; }
    }
}
