namespace Common.Configuration
{
    public record DatabaseSettings
    {
        public const string Name = nameof(DatabaseSettings);

        public string? ConnectionString { get; init; }
    }
}
