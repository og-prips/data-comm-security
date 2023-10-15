using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Assignment4.WebApi.Hubs
{
    // Hubben som används för att skicka meddelandet från controllern, för att ansluta till hubben krävs att man är auktoriserad med policyn "ReadPermission"
    [Authorize(Policy = "ReadPermission")]
    public class TemperatureHub : Hub
    {
    }
}
