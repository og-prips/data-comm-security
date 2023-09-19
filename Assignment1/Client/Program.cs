using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Skapa en klient och anslut till servern
            TcpClient client = new TcpClient();
            int port = 1024; // Använd samma portnummer som servern
            string serverIp = "127.0.0.1"; // Ersätt med serverns IP-adress

            client.Connect(serverIp, port);
            Console.WriteLine("Connected to the server.");

            // Skicka data till servern
            Console.WriteLine("Send a message to the server:");

            string message = Console.ReadLine();
            byte[] messageBytes = Encoding.ASCII.GetBytes(message);
            NetworkStream stream = client.GetStream();
            stream.Write(messageBytes, 0, messageBytes.Length);
            Console.WriteLine("Message sent to server: " + message);

            // Ta emot svar från servern
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string serverResponse = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Server response: " + serverResponse);

            Console.ReadLine();

            // Stäng klienten

        }
    }
}