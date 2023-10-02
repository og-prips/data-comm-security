using Assignment1.Common.DTOs;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Skapa en klient och anslut till servern
            TcpClient client = new TcpClient();
            int port = 1024;
            string serverIp = "127.0.0.1";

            client.Connect(serverIp, port);
            Console.WriteLine("Connected to the server.");

            // Hämta klientens ström
            NetworkStream stream = client.GetStream();

            Console.WriteLine("Send a message to the server:");
            string input = Console.ReadLine()!;

            // Loopa till dess att klienten vill avbryta
            while (input != "stop")
            {
                // Skapar ett meddelande från inmatning i konsollen som sedan serialiseras och skickas till strömmen
                Message message = new(input!, DateTime.Now);

                string jsonMessage = JsonSerializer.Serialize(message);
                byte[] messageBytes = Encoding.UTF8.GetBytes(jsonMessage);

                stream.Write(messageBytes, 0, messageBytes.Length);
                Console.WriteLine("Message sent to server: " + jsonMessage);

                // Ta emot svar från servern
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string serverResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                // Deserialisera mottaget meddelande
                Message? receivedMessage = JsonSerializer.Deserialize<Message>(serverResponse);

                Console.WriteLine($"{receivedMessage!.DateSent} - Server responded with: {receivedMessage.Text}");

                Console.WriteLine("Send another message or exit with 'stop'");
                input = Console.ReadLine()!;
            }

            // Stäng klienten
            client.Close();
        }
    }
}