using Meme.Hub.TokenCreationListenerWebJob.Services;
using System.Net.WebSockets;
using System.Runtime;
using System.Text;

namespace Meme.Hub.TokenCreationListenerWebJob
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("WebJob started.");

            var serviceBusQueueWriter = new ServiceBusQueueWriter();

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

                try
                {
                    using (var webSocket = new ClientWebSocket())
                    {
                        Uri serverUri = new Uri("wss://pumpportal.fun/api/data");
                        try
                        {
                            await webSocket.ConnectAsync(serverUri, CancellationToken.None);
                            Console.WriteLine("Connected to the WebSocket server");

                            // Subscribing to token creation events
                            await SendMessage(webSocket, new { method = "subscribeNewToken" });

                            Task receiveTask = ReceiveMessages(webSocket, serviceBusQueueWriter);
                            await receiveTask;
                        }
                        catch (WebSocketException e) { Console.WriteLine($"WebSocket error: {e.Message}"); }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.ToString());
                }

                await Task.Delay(TimeSpan.FromMinutes(10), cts.Token);
            }

            Console.WriteLine("WebJob stopped.");
        }

        static async Task SendMessage(ClientWebSocket webSocket, object payload)
        {
            string message = System.Text.Json.JsonSerializer.Serialize(payload);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        static async Task ReceiveMessages(ClientWebSocket webSocket, ServiceBusQueueWriter serviceBusQueueWriter)
        {
            byte[] buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                await serviceBusQueueWriter.SendMessageAsync(message);

                Console.WriteLine($"Received: {message}");
            }
        }
    }
}
