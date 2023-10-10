using Assignment4.IoTTempSimulator.Models;
using Assignment4.IoTTempSimulator.Services;

namespace Assignment4.IoTTempSimulator.Simulators
{
    internal class IoTSimulator
    {
        private readonly TemperatureDataManager _temperatureDataManager;

        private string _deviceId;
        private double _minTemp;
        private double _maxTemp;
        private int _minHumidity;
        private int _maxHumidity;

        public IoTSimulator(
            TemperatureDataManager signalRManager,
            string deviceId,
            double minTemp,
            double maxTemp,
            int minHumidity,
            int maxHumidity)
        {
            _temperatureDataManager = signalRManager;
            this._deviceId = deviceId;
            this._minTemp = minTemp;
            this._maxTemp = maxTemp;
            this._minHumidity = minHumidity;
            this._maxHumidity = maxHumidity;
        }

        public async Task StartSimulationAsync()
        {
            Console.WriteLine("Temperature Sensor Simulator is running. Press Ctrl+C to exit.");

            while (true)
            {
                var temperatureData = GetRandomTemperatureData();
                Console.WriteLine($"Sending temperature: {temperatureData.Temperature}°C");

                await _temperatureDataManager.SendTemperatureAsync(temperatureData);
                await Task.Delay(5000);
            }
        }

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
