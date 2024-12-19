using Meme.Domain.Models.Settings;
using Meme.Hub.PumpFunListener.Models;
using Meme.Hub.PumpFunListener.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Meme.Hub.PumpFunListener
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.Configure<PumpPortalSettings>(
                builder.Configuration.GetSection("PumpPortalSettings"));

            builder.Services.Configure<MessagingBusSettings>(
                builder.Configuration.GetSection("MessagingBus"));

            builder.Services.AddControllers();

            // Register Hosted Services
            builder.Services.AddHostedService<BackgroundListener>();
            builder.Services.AddSingleton<IPumpPortalClient, PumpPortalClient>();

            builder.Services.AddSingleton<IServiceBusQueueWriter, ServiceBusQueueWriter>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
