using Azure.Messaging.ServiceBus;
using Meme.Hub.WebJob.Common;
using System.Net.WebSockets;

namespace Meme.Hub.TokenRawDataProcessor.WebJob
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("WebJob started.");

            ServiceBusClient _client = new ServiceBusClient(Settings.serviceBusConnectionString);
            ServiceBusProcessor _processor = _client.CreateProcessor(Settings.queueName, new ServiceBusProcessorOptions());
            

            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("Shutting down...");
                cts.Cancel();
                e.Cancel = true;
            };

            while (!cts.Token.IsCancellationRequested)
            {
                // Your long-running task logic here
                Console.WriteLine($"Listener Webjob Running at {DateTime.Now}");

                _processor.ProcessMessageAsync += ProcessMessagesAsync;
                _processor.ProcessErrorAsync += ProcessErrorAsync;

                await _processor.StartProcessingAsync();

                Console.WriteLine("Azure Service Bus Queue Listener started.");

                await Task.Delay(TimeSpan.FromMinutes(10), cts.Token);
            }

            Console.WriteLine("WebJob stopped.");
        }

        private static async Task ProcessMessagesAsync(ProcessMessageEventArgs args)
        {
            RawDataProcessor _tokenRawDataProcessor = new();
            string messageBody = args.Message.Body.ToString();
            await _tokenRawDataProcessor.ProcessTokenAsync(messageBody);

            // Complete the message. Message is deleted from the queue.
            await args.CompleteMessageAsync(args.Message);
        }

        private static Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.Message, "Message handler encountered an exception");
            return Task.CompletedTask;
        }
    }
}
