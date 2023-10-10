using Assignment4.IoTTempSimulator.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Assignment4.IoTTempSimulator.Services
{
    internal class TemperatureDataManager
    {
        private readonly IConfiguration _config;
        private readonly EncryptionService _encryptionService;
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public TemperatureDataManager(IConfiguration config, EncryptionService encryptionService)
        {
            _config = config;
            _encryptionService = encryptionService;
            _httpClient = new HttpClient();
            _apiUrl = _config.GetValue<string>("ApiUrl")!;
        }

        public async Task SendTemperatureAsync(TemperatureData temperatureData)
        {
            try
            {
                string jsonPayload = JsonSerializer.Serialize(temperatureData);
                byte[] encryptedTemperatureData = _encryptionService.EncryptStringToBytes_Aes(jsonPayload);
                string base64EncryptedTemperatureData = Convert.ToBase64String(encryptedTemperatureData);

                var content = new StringContent(JsonSerializer.Serialize(base64EncryptedTemperatureData), System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Temperature data sent successfully: {temperatureData.Temperature}°C");
                }
                else
                {
                    Console.WriteLine($"Error sending temperature data: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the request
                Console.WriteLine($"Error sending temperature data: {ex.Message}");
            }
        }
    }
}
