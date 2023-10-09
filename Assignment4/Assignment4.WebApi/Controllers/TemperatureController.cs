using Assignment4.WebApi.Hubs;
using Assignment4.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Assignment4.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemperatureController : ControllerBase
    {
        private readonly IHubContext<TemperatureHub> _temperatureHubContext;

        public TemperatureController(IHubContext<TemperatureHub> temperatureHubContext)
        {
            _temperatureHubContext = temperatureHubContext;
        }

        [HttpPost]
        public async Task<IActionResult> SendTemperatureAsync([FromBody] TemperatureData temperatureData)
        {
            Console.WriteLine(temperatureData.Temperature);

            if (ModelState.IsValid)
            {
                await _temperatureHubContext.Clients.All.SendAsync("SendTemperature", temperatureData);

                return Ok();
            }

            return BadRequest();
        }
    }
}
