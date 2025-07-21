namespace Common.Configuration
{
    public record DatabaseSettings
    {
        public const string Name = nameof(DatabaseSettings);

        public string? ConnectionString { get; init; }

        public string? DatabaseName { get; init; }

        public string? CollectionName { get; init; }

        /// <summary>
        /// Specifies if <see cref="ConnectionString"/> provided points to Mongo Atlas, which enables Atlas Search Database Commands for example.
        /// When using this configuration, API can provide search and autocomplete funcitonality for example, which is not available in the standard mongo instance.
        /// </summary>
        public bool UseAtlas { get; init; }
    }
}