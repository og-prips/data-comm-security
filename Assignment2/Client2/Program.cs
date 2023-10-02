using Assignment2.Common.DTOs;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Client2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int listenPort = 11001;
            int remoteHostPort = 11000;
            string remoteHostName = "127.0.0.1";

            UdpClient udpClient = new UdpClient();
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            // Start a background thread to continuously receive messages
            var receiveThread = new Thread(() =>
            {
                UdpClient listener = new UdpClient(listenPort);
                try
                {
                    while (true)
                    {
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
            receiveThread.Start();

            Console.WriteLine($"Client is running, messages are now being sent to {remoteHostName}");

            // Start a loop to send messages
            while (true)
            {
                string input = Console.ReadLine()!;

                Message message = new Message(input, DateTime.Now);
                string jsonMessage = JsonSerializer.Serialize(message);

                byte[] sendBytes = Encoding.UTF8.GetBytes(jsonMessage);

                udpClient.Send(sendBytes, sendBytes.Length, remoteHostName, remoteHostPort); // Send to Client1
            }
        }
    }
}