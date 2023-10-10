using Assignment4.WebApi.DTOs;
using Assignment4.WebApi.Hubs;
using Assignment4.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Assignment4.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemperatureController : ControllerBase
    {
        private readonly IHubContext<TemperatureHub> _temperatureHubContext;
        private readonly DecryptionService _decryptionService;

        public TemperatureController(IHubContext<TemperatureHub> temperatureHubContext, DecryptionService decryptionService)
        {
            _temperatureHubContext = temperatureHubContext;
            _decryptionService = decryptionService;
        }

        [HttpPost]
        public async Task<IActionResult> SendTemperatureAsync([FromBody] string encryptedTemperatureDataRequest)
        {
            byte[] base64EncryptedData = Convert.FromBase64String(encryptedTemperatureDataRequest);
            string decryptedData = _decryptionService.DecryptStringFromBytes_Aes(base64EncryptedData);

            TemperatureDataRequest request = JsonSerializer.Deserialize<TemperatureDataRequest>(decryptedData)!;

            var validationContext = new ValidationContext(request, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(request, validationContext, validationResults, validateAllProperties: true))
            {
                foreach (var validationResult in validationResults)
                {
                    foreach (var memberName in validationResult.MemberNames)
                    {
                        ModelState.AddModelError(memberName, validationResult.ErrorMessage);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                await _temperatureHubContext.Clients.All.SendAsync("ReceiveTemperatureData", request);

                return Ok();
            }

            return BadRequest();
        }
    }
}
