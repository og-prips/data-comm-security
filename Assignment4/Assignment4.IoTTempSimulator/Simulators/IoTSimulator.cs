using Assignment4.IoTTempSimulator.Models;
using Assignment4.IoTTempSimulator.Services;

namespace Assignment4.IoTTempSimulator.Simulators
{
    internal class IoTSimulator
    {
        private readonly TemperatureDataManager _temperatureDataManager;

        public IoTSimulator(TemperatureDataManager signalRManager)
        {
            _temperatureDataManager = signalRManager;
        }

        public async Task StartSimulationAsync()
        {
            Console.WriteLine("Temperature Sensor Simulator is running. Press Ctrl+C to exit.");

            while (true)
            {
                double minValue = -10.0;
                double maxValue = 30.0;
                double temperature = minValue + (new Random().NextDouble() * (maxValue - minValue));

                var temperatureData = new TemperatureData { Temperature = temperature };
                Console.WriteLine($"Sending temperature: {temperature}°C");

                // Send the temperature value to the server via SignalR
                await _temperatureDataManager.SendTemperatureAsync(temperatureData);

                // Wait for 5 seconds before the next simulation
                await Task.Delay(5000);
            }
        }
    }
}
