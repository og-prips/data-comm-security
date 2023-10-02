using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
                await StartListenerAsync();
            }
        }

        static async Task StartListenerAsync()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:11000/");
            listener.Start();

            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for connection...");

                    HttpListenerContext context = await listener.GetContextAsync();
                    if (context.Request.IsWebSocketRequest)
                    {
                        WebSocketContext wsContext = await context.AcceptWebSocketAsync(null);
                        Console.WriteLine("Connected to client.");

                        WebSocket webSocket = wsContext.WebSocket;
                        await HandleClient(webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                    }
                }
            }
            catch (HttpListenerException ex)
            {
                Console.WriteLine($"Listener exception: {ex.Message}");
            }
            finally
            {
                listener.Close();
            }
        }

        static async Task HandleClient(WebSocket webSocket)
        {
            byte[] buffer = new byte[1024];
            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine($"Message from client: {message}");

                        // Skicka svar till klienten
                        string response = $"Message received.";
                        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                        await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Client connection closed: {ex.Message}");
                    break; // Exit the loop when the client connection is closed.
                }
            }
        }
    }
}