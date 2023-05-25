namespace PaymentService
{
    public class PaymentWorker : BackgroundService
    {
        private readonly ILogger<PaymentWorker> _logger;

        public PaymentWorker(ILogger<PaymentWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}