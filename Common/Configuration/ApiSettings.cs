namespace Common.Configuration
{
    public record ApiSettings
    {
        public const string Name = nameof(ApiSettings);

        public string GatewayAddress { get; init; }
    }
}
