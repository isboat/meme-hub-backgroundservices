
using Meme.Domain.Models.Settings;
using Microsoft.Extensions.Options;
using System.Runtime;

namespace Meme.Hub.PumpFunListener.Services
{
    public class BackgroundListener : BackgroundService
    {
        private readonly ILogger<BackgroundListener> _logger;
        private readonly MessagingBusSettings _messagingBusSettings;
        private readonly IPumpPortalClient _pumpPortalClient;

        //private readonly IEventBus _eventBus;

        public BackgroundListener(ILogger<BackgroundListener> logger,
                                     IPumpPortalClient pumpPortalClient)
        {
            _logger = logger;
            _pumpPortalClient = pumpPortalClient;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //_logger.LogDebug($"GracePeriodManagerService is starting.");

            //stoppingToken.Register(() =>_logger.LogDebug($" GracePeriod background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("backgroun service is runing");
                await _pumpPortalClient.Get();

                try
                {
                    await Task.Delay(50000, stoppingToken);
                }
                catch (TaskCanceledException exception)
                {
                    Console.WriteLine(exception.Message);
                    //_logger.LogCritical(exception, "TaskCanceledException Error", exception.Message);
                }
                catch (Exception exception) { Console.WriteLine(exception.Message); }
            }

            //_logger.LogDebug($"GracePeriod background task is stopping.");
        }
    }
}
