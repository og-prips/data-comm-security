using System.Net.WebSockets;
using System.Text;

namespace Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
                try
                {
                    await Run();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    // Add any additional error handling or logging here.

                    // Sleep for a while before attempting reconnection.
                    Thread.Sleep(TimeSpan.FromSeconds(5)); // Adjust the delay as needed.
                }
            }
        }

        static async Task Run()
        {
            using (ClientWebSocket clientWebSocket = new ClientWebSocket())
            {
                Uri serverUri = new Uri("ws://localhost:11000/");
                await clientWebSocket.ConnectAsync(serverUri, CancellationToken.None);
                Console.WriteLine("Connected to server.");

                while (clientWebSocket.State == WebSocketState.Open)
                {
                    Console.Write("Send a message to the server: ");
                    string message = Console.ReadLine();
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);

                    await clientWebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);

                    // Mottag och visa svar från servern
                    byte[] buffer = new byte[1024];
                    WebSocketReceiveResult result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    string response = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Server responded with: {response}");
                }
            }
        }
    }
}