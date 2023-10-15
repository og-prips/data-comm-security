using Assignment4.IoTTempSimulator.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Assignment4.IoTTempSimulator.Services
{
    internal class TemperatureDataManager
    {
        private readonly IConfiguration _config;
        private readonly EncryptionService _encryptionService;
        private readonly string _apiUrl;

        public TemperatureDataManager(IConfiguration config, EncryptionService encryptionService)
        {
            _config = config;
            _encryptionService = encryptionService;
            _apiUrl = _config.GetValue<string>("ApiUrl")! + "temperature";
        }

        /// <summary>
        /// Denna metod används för att skicka temperaturdata till API:et och kräver en JWT för att utföra detta.
        /// Först krypteras och serialiseras datan och skickas sedan via HTTPS med JWT som Header för auktorisering.
        /// </summary>
        /// <param name="temperatureData"></param>
        /// <param name="authToken"></param>
        /// <returns></returns>
        public async Task SendTemperatureDataAsync(TemperatureData temperatureData, HttpClient httpClient)
        {
            try
            {
                string encryptedTemperatureData = EncryptTemperatureData(temperatureData);
                var content = new StringContent(JsonSerializer.Serialize(encryptedTemperatureData), System.Text.Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(_apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    await Console.Out.WriteLineAsync($"{temperatureData.DeviceId}: Temperature data sent successfully.\n");
                }
                else
                {
                    await Console.Out.WriteLineAsync($"{temperatureData.DeviceId}: Error sending temperature data, server responded with: {response.StatusCode}\n");

                    string errorContent = await response.Content?.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(errorContent))
                    {
                        await Console.Out.WriteLineAsync($"{temperatureData.DeviceId}: API error response: {errorContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"{temperatureData.DeviceId}: ExceptionError sending temperature data: {ex.Message}");
            }
        }

        /// <summary>
        /// Krypterar ett TemperatureData objekt genom att förstr serializera till JSON och sen kryptera det med hjälp utav metoden i EncryptionService.
        /// Returnerar den krypterade datan i form av en Base64String.
        /// </summary>
        /// <param name="temperatureData"></param>
        /// <returns></returns>
        private string EncryptTemperatureData(TemperatureData temperatureData)
        {
            string jsonPayload = JsonSerializer.Serialize(temperatureData);
            byte[] encryptedTemperatureData = _encryptionService.EncryptStringToBytes_Aes(jsonPayload);
            return Convert.ToBase64String(encryptedTemperatureData);
        }
    }
}
