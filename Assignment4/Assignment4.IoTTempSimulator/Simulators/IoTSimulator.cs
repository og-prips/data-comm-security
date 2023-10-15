using Assignment4.IoTTempSimulator.Models;
using Assignment4.IoTTempSimulator.Services;
using System.Net.Http.Headers;

namespace Assignment4.IoTTempSimulator.Simulators
{
    internal class IoTSimulator
    {
        private readonly AuthService _authService;
        private readonly TemperatureDataManager _temperatureDataManager;

        private string _deviceId;
        private string _identityName;
        private double _minTemp;
        private double _maxTemp;
        private int _minHumidity;
        private int _maxHumidity;

        public IoTSimulator(
            AuthService authService,
            TemperatureDataManager temperatureDataManager,
            string deviceId,
            string identityName,
            double minTemp,
            double maxTemp,
            int minHumidity,
            int maxHumidity)
        {
            _authService = authService;
            _temperatureDataManager = temperatureDataManager;
            _deviceId = deviceId;
            _identityName = identityName;
            _minTemp = minTemp;
            _maxTemp = maxTemp;
            _minHumidity = minHumidity;
            _maxHumidity = maxHumidity;
        }

        /// <summary>
        /// En metod för att kontinuerligt skicka temperatur-data till API:et.
        /// Först hämtas en JWT och om den är giltig körs själva loopen som skickar datat var 5e sekund.
        /// </summary>
        /// <returns></returns>
        public async Task StartSimulationAsync()
        {
            await Console.Out.WriteLineAsync($"Temperature Sensor Simulator '{_deviceId}' is running. Press Ctrl+C to exit.");

            await Console.Out.WriteLineAsync($"{_deviceId}: Attempting to authorize as '{_identityName}'\n");

            string authToken = await _authService.Login(_identityName);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            if (string.IsNullOrEmpty(authToken))
            {
                await Console.Out.WriteLineAsync($"{_deviceId}: Failed to authenticate with identity '{_identityName}'");
                return;
            }

            await Console.Out.WriteLineAsync($"{_deviceId} Authorized.\n");

            while (true)
            {
                var temperatureData = GetRandomTemperatureData();
                await Console.Out.WriteLineAsync($"{_deviceId}: Sending temperature: {temperatureData.Temperature}°C\n");

                await _temperatureDataManager.SendTemperatureDataAsync(temperatureData, httpClient);
                await Task.Delay(5000);
            }
        }

        /// <summary>
        /// Metod för att hämta slumpat väderdata baserat på vad som angetts i konstruktorn.
        /// </summary>
        /// <returns></returns>
        private TemperatureData GetRandomTemperatureData()
        {
            double temperature = _minTemp + (new Random().NextDouble() * (_maxTemp - _minTemp));
            int humidity = new Random().Next(_minHumidity, _maxHumidity);

            return new TemperatureData
            {
                DeviceId = _deviceId,
                Temperature = temperature,
                Humidity = humidity,
                DateSent = DateTime.Now
            };
        }
    }
}
