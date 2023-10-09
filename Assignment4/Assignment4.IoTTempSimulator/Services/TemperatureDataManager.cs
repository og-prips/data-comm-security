using Assignment4.IoTTempSimulator.Models;
using System.Text.Json;

namespace Assignment4.IoTTempSimulator.Services
{
    internal class TemperatureDataManager
    {
        private readonly HttpClient _httpClient;

        public TemperatureDataManager()
        {
            _httpClient = new HttpClient();
        }

        public async Task SendTemperatureAsync(TemperatureData temperatureData)
        {
            // Modify this method to send temperature data via HTTP to your API
            // Create an HttpClient and make an HTTP POST request to the API endpoint
            // You will need to specify the API endpoint URL, headers, and content

            var apiUrl = "https://localhost:7205/api/temperature"; // Replace with your API endpoint URL

            try
            {
                var jsonPayload = JsonSerializer.Serialize(temperatureData);
                var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

                // Make the HTTP POST request
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // Request was successful
                    Console.WriteLine($"Temperature data sent successfully: {temperatureData.Temperature}°C");
                }
                else
                {
                    // Handle errors if the request fails
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
