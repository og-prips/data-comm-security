using Assignment2.Common.DTOs;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Client1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int listenPort = 11000;
            int remoteHostPort = 11001;
            string remoteHostName = "127.0.0.1";

            // Klient för att skicka meddelanden
            UdpClient udpClient = new UdpClient();

            // IPEndPoint som lyssnaren använder för att ta emot meddelanden från alla IP-adresser på angiven port
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            // En tråd som kontinuerligt tar emot meddelanden
            // Tråden fångar upp ett eventuellt Socket-fel och stänger isåfall ner klienten
            var receiveThread = new Thread(() =>
            {
                UdpClient listener = new UdpClient(listenPort);
                try
                {
                    while (true)
                    {
                        // Tar emot och deserialiserar meddelandet som sedan skrivs ut i konsollen
                        byte[] bytes = listener.Receive(ref groupEP);
                        Message? recievedMessage = JsonSerializer.Deserialize<Message>(bytes);

                        Console.WriteLine($"Message recieved from: {groupEP}:");
                        Console.WriteLine($"{recievedMessage!.DateSent} - {recievedMessage.Text}");
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    listener.Close();
                }
            });
            // Startar tråden
            receiveThread.Start();

            Console.WriteLine($"Client is running, messages are now being sent to {remoteHostName}");

            // Startar en loop för att ta emot användarens input och skickar som ett serialiserat meddelande till mottagande part
            while (true)
            {
                string input = Console.ReadLine()!;

                Message message = new Message(input, DateTime.Now);
                string jsonMessage = JsonSerializer.Serialize(message);

                byte[] sendBytes = Encoding.UTF8.GetBytes(jsonMessage);

                udpClient.Send(sendBytes, sendBytes.Length, remoteHostName, remoteHostPort); // Skickar till Client2
            }
        }
    }
}