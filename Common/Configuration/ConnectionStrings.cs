namespace Common.Configuration
{
    public record ConnectionStrings
    {
        public const string Name = nameof(ConnectionStrings);

        public required string AMQPConnectionString { get; set; }
    }
}
