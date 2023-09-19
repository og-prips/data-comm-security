using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int port = 1024;
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            Console.WriteLine("Server is up and running, listening on port: " + port);

            // Vänta på klientanslutningar
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected.");

            // Skapa en tråd för att hantera kommunikation med klienten
            Thread clientThread = new Thread(HandleClient);
            clientThread.Start(client);

            Console.ReadLine();
        }

        // Metod för att hantera kommunikation med klienten
        static void HandleClient(object clientObj)
        {
            TcpClient client = (TcpClient)clientObj;

            // Hämta klientens ström
            NetworkStream stream = client.GetStream();

            // Läs data från klienten
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string clientData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Client sent: " + clientData);

            // Skicka svar till klienten
            string response = "Hello from the server!";
            byte[] responseData = Encoding.ASCII.GetBytes(response);
            stream.Write(responseData, 0, responseData.Length);

            // Stäng anslutningen till klienten
        }
    }
}