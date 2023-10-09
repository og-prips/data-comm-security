using Assignment3.Common.DTOs;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
                // Klienten körs i en try/catch för att inte programmen ska krascha ifall anslutningen skulle upphöra
                try
                {
                    await Run();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    Console.WriteLine("Reconnecting...");

                    // Vänta fem sekunder innan vi försöker ansluta igen
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
            }
        }

        // Metod för att skicka/ta emot meddelanden till server via websockets
        static async Task Run()
        {
            using (var clientWebSocket = new ClientWebSocket())
            {
                // Sätt upp Uri och anslut till servern
                var serverUri = new Uri("ws://localhost:11000/");

                await clientWebSocket.ConnectAsync(serverUri, CancellationToken.None);
                Console.WriteLine("Connected to server.");

                // Loopa medan anslutningen är öppen
                while (clientWebSocket.State == WebSocketState.Open)
                {
                    Console.Write("Send a message to the server: ");
                    string messageText = Console.ReadLine()!;

                    if (string.IsNullOrWhiteSpace(messageText)) continue;

                    Message message = new(messageText, DateTime.Now);
                    string jsonMessage = JsonSerializer.Serialize(message);

                    byte[] messageBytes = Encoding.UTF8.GetBytes(jsonMessage);

                    // Skicka serialiserat meddelande till serven via websocket
                    await clientWebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);

                    // Mottag och visa svar från servern
                    byte[] buffer = new byte[1024];
                    WebSocketReceiveResult result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    string jsonResponse = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Message? responseMessage = JsonSerializer.Deserialize<Message>(jsonResponse);

                    Console.WriteLine($"{responseMessage!.DateSent} - Server responded with: {responseMessage.Text}");
                }
            }
        }
    }
}