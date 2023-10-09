using Assignment3.Common.DTOs;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

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

        // Metod för att starta lyssnaren och hantera eventuella anslutningsfel
        static async Task StartListenerAsync()
        {
            // Sätt upp en http listner som hostas på localhost
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

        // Metod för att skicka/ta emot svar från klienten. Körs sålänge WebSocket-anslutningen är öppen
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
                        // Ta emot svar från klienten
                        string recievedJsonMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Message? recievedMessage = JsonSerializer.Deserialize<Message>(recievedJsonMessage);

                        Console.WriteLine($"{recievedMessage.DateSent} - Message from client: {recievedMessage!.Text}");

                        // Skicka svar till klienten
                        Message responseMessage = new("Message recieved.", DateTime.Now);
                        string jsonResponse = JsonSerializer.Serialize(responseMessage);
                        byte[] responseBytes = Encoding.UTF8.GetBytes(jsonResponse);

                        await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Client connection closed: {ex.Message}");
                    break; // Stoppa loopen när klientens anslutning stängs
                }
            }
        }
    }
}