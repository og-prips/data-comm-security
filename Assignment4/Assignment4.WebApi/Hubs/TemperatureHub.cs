using Microsoft.AspNetCore.SignalR;

namespace Assignment4.WebApi.Hubs
{
    public class TemperatureHub : Hub
    {
        public async Task SendTemperature(int temperature)
        {
            await Clients.All.SendAsync("ReceiveTemperature", temperature);
        }
    }
}
