using Assignment1.Common.DTOs;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Skapa en lyssnare som lyssnar på alla adresser på det lokala nätverket
            int port = 11000;
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            Console.WriteLine("Server is up and running, listening on port: " + port);

            // Vänta på klientanslutningar
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected.");

            // Skapa en tråd för att hantera kommunikation med klienten
            Thread clientThread = new Thread(start: HandleClient!);
            clientThread.Start(client);
        }

        // Metod för att hantera kommunikation med klienten
        static void HandleClient(object clientObj)
        {
            // Hämta klienten och klientens ström
            TcpClient client = (TcpClient)clientObj;
            NetworkStream stream = client.GetStream();

            // Loopar och lyssnar/svarar på klienten till dess att anslutningen upphör
            try
            {
                while (true)
                {
                    // Läs data från klienten
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string clientData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // Deserialisera mottaget meddelande
                    Message? receivedMessage = JsonSerializer.Deserialize<Message>(clientData);

                    Console.WriteLine($"{receivedMessage!.DateSent} - Message recieved from client: {receivedMessage.Text}");

                    // Skicka svar till klienten
                    Message responseMessage = new("Hello from the server!", DateTime.Now);

                    string jsonResponseMessage = JsonSerializer.Serialize(responseMessage);
                    byte[] messageBytes = Encoding.UTF8.GetBytes(jsonResponseMessage);
                    stream.Write(messageBytes, 0, messageBytes.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Shutting down...");
            }
        }
    }
}