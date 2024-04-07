namespace Common.Configuration
{
    public record RabbitMQOptions
    {
        public const string Name = "RabbitMQ";

        public string AMQPConnectionString { get; set; }
    }
}
