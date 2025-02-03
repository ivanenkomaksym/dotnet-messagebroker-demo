namespace Common.Configuration
{
    public class ApplicationOptions
    {
        public const string Name = nameof(ApplicationOptions);

        public StartupEnvironment StartupEnvironment { get; set; }
    }

}