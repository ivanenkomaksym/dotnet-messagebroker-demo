namespace Common.Configuration
{
    public record DatabaseSettings
    {
        public const string Name = nameof(DatabaseSettings);

        public string? ConnectionString { get; init; }

        public string? DatabaseName { get; init; }

        public string? CollectionName { get; init; }
    }
}
