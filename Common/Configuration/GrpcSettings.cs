namespace Common.Configuration
{
    public record GrpcSettings
    {
        public const string Name = nameof(GrpcSettings);

        public string? OrderGrpcUrl { get; init; }
    }
}
