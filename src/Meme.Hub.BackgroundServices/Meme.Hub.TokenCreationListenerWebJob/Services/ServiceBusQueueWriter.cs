
using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Meme.Hub.WebJob.Common;

namespace Meme.Hub.TokenCreationListenerWebJob.Services
{
    public interface IServiceBusQueueWriter
    {
        Task SendMessageAsync(string messageBody);
        Task SendMessageBatchAsync(string[] messageBodies);
    }

    public delegate IServiceBusQueueWriter ServiceResolver(string key);

    public class ServiceBusQueueWriter: IServiceBusQueueWriter
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;

        public ServiceBusQueueWriter()
        {
            _client = new ServiceBusClient(Settings.serviceBusConnectionString);
            _sender = _client.CreateSender(Settings.queueName);
        }

        public async Task SendMessageAsync(string messageBody)
        {
            if (string.IsNullOrEmpty(messageBody))
            {
                throw new ArgumentException("Message body cannot be null or empty", nameof(messageBody));
            }

            ServiceBusMessage message = new ServiceBusMessage(messageBody);
            await _sender.SendMessageAsync(message);
            Console.WriteLine($"Sent message: {messageBody}");
        }

        public async Task SendMessageBatchAsync(string[] messageBodies)
        {
            if (messageBodies == null || messageBodies.Length == 0)
            {
                throw new ArgumentException("Message bodies cannot be null or empty", nameof(messageBodies));
            }

            using ServiceBusMessageBatch messageBatch = await _sender.CreateMessageBatchAsync();

            foreach (var messageBody in messageBodies)
            {
                if (!messageBatch.TryAddMessage(new ServiceBusMessage(messageBody)))
                {
                    throw new Exception($"The message {messageBody} is too large to fit in the batch.");
                }
            }

            await _sender.SendMessagesAsync(messageBatch);
            Console.WriteLine($"Sent a batch of {messageBodies.Length} messages");
        }

        public async Task CloseAsync()
        {
            await _sender.CloseAsync();
            await _client.DisposeAsync();
        }
    }

}
