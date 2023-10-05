using Microsoft.AspNetCore.SignalR;

namespace Assignment4.WebClient.Hubs
{
    public class TemperatureHub : Hub
    {
        public async Task SendTemperature(int temperature)
        {
            // Your code here to handle the received temperature data
            await Clients.All.SendAsync("ReceiveTemperature", temperature);
        }
    }
}
