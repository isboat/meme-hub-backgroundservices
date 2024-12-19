using Meme.Hub.PumpFunListener.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;

namespace Meme.Hub.PumpFunListener.Services
{

    public interface IPumpPortalClient
    {
        Task Get();
    }

    public class PumpPortalClient: IPumpPortalClient
    {
        private readonly HttpClient _httpClient;
        private readonly PumpPortalSettings _settings;
        private readonly IServiceBusQueueWriter _serviceBusQueueWriter;

        public PumpPortalClient(IHttpClientFactory httpClientFactory, 
            IOptions<PumpPortalSettings> settings,
            [FromKeyedServices("tokencreationrawdata")] IServiceBusQueueWriter serviceBusQueueWriter)
        {
            _settings = settings.Value;
            _serviceBusQueueWriter = serviceBusQueueWriter;
        }

        public async Task Get()
        {
            try
            {
                using (var webSocket = new ClientWebSocket())
                {
                    Uri serverUri = new Uri(_settings.BaseUrl!);
                    try
                    {
                        await webSocket.ConnectAsync(serverUri, CancellationToken.None);
                        Console.WriteLine("Connected to the WebSocket server");

                        // Subscribing to token creation events
                        await SendMessage(webSocket, new { method = "subscribeNewToken" });

                        Task receiveTask = ReceiveMessages(webSocket);
                        await receiveTask;
                    }
                    catch (WebSocketException e) { Console.WriteLine($"WebSocket error: {e.Message}"); }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString());
            }

        }

        async Task SendMessage(ClientWebSocket webSocket, object payload) 
        { 
            string message = System.Text.Json.JsonSerializer.Serialize(payload); 
            byte[] messageBytes = Encoding.UTF8.GetBytes(message); 
            await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None); 
        }

        async Task ReceiveMessages(ClientWebSocket webSocket) 
        { 
            byte[] buffer = new byte[1024 * 4]; 
            while (webSocket.State == WebSocketState.Open) 
            { 
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None); 
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count); 

                await _serviceBusQueueWriter.SendMessageAsync(message);

                Console.WriteLine($"Received: {message}"); 
            } 
        }
    }
}
