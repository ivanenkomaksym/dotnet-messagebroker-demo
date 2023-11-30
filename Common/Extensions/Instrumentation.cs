using System.Diagnostics.Metrics;
using System.Diagnostics;

namespace Common.Extensions
{
    /// <summary>
    /// It is recommended to use a custom type to hold references for
    /// ActivitySource and Instruments. This avoids possible type collisions
    /// with other components in the DI container.
    /// </summary>
    public class Instrumentation : IDisposable
    {
        internal const string ActivitySourceName = "EShopActivity";
        internal const string MeterName = "EShopActivity";
        private readonly Meter meter;

        public Instrumentation()
        {
            string? version = typeof(Instrumentation).Assembly.GetName().Version?.ToString();
            this.ActivitySource = new ActivitySource(ActivitySourceName, version);
            this.meter = new Meter(MeterName, version);
        }

        public ActivitySource ActivitySource { get; }

        public void Dispose()
        {
            this.ActivitySource.Dispose();
            this.meter.Dispose();
        }
    }
}
