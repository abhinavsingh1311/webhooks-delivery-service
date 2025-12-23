using System.Diagnostics.Tracing;
using Webhooks_System_Library.Repositories;

namespace WebHooks.Workers
{
    public class DeliveryWorker : BackgroundService
    {
        private IServiceProvider _serviceProvider;
        private readonly ILogger<DeliveryWorker> _logger;
        private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(5);

        public DeliveryWorker(IServiceProvider serviceProvider, ILogger<DeliveryWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DeliveryWorker started at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var deliveryScope = scope.ServiceProvider.GetRequiredService<IDeliveryService>();
                    await deliveryScope.ProcessPendingEventsAsync();
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing pending events in DeliveryWorker.");
                }
            await Task.Delay(_pollingInterval, stoppingToken);
            }

            _logger.LogInformation("DeliveryWorker stopped at: {time}", DateTimeOffset.Now);
        }
}
