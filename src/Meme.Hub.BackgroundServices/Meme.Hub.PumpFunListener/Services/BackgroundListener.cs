
using Meme.Domain.Models.Settings;
using Microsoft.Extensions.Options;
using System.Runtime;

namespace Meme.Hub.PumpFunListener.Services
{
    public class BackgroundListener : BackgroundService
    {
        private readonly ILogger<BackgroundListener> _logger;
        private readonly SolanaDataSourceSettings _solanaDataSourceSettings;
        private readonly MessagingBusSettings _messagingBusSettings;

        //private readonly IEventBus _eventBus;

        public BackgroundListener(IOptions<SolanaDataSourceSettings> solanaDataSourceSettings,
            IOptions<MessagingBusSettings> messagingBusSettings,
                                     //IEventBus eventBus,
                                     ILogger<BackgroundListener> logger)
        {
            _logger = logger;
            _solanaDataSourceSettings = solanaDataSourceSettings.Value;
            _messagingBusSettings = messagingBusSettings.Value;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //_logger.LogDebug($"GracePeriodManagerService is starting.");

            //stoppingToken.Register(() =>_logger.LogDebug($" GracePeriod background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("backgroun service is runing");
                //_logger.LogDebug($"GracePeriod task doing background work.");

                // This eShopOnContainers method is querying a database table
                // and publishing events into the Event Bus (RabbitMQ / ServiceBus)
                //CheckConfirmedGracePeriodOrders();

                try
                {
                   await Task.Delay(500, stoppingToken);
                }
                catch (TaskCanceledException exception)
                {
                    //_logger.LogCritical(exception, "TaskCanceledException Error", exception.Message);
                }
            }

            //_logger.LogDebug($"GracePeriod background task is stopping.");
        }
    }
}
