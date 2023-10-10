using Assignment4.IoTTempSimulator.Services;
using Assignment4.IoTTempSimulator.Simulators;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("config.json", optional: false, reloadOnChange: true);

IConfiguration config = builder.Build();

var encryptionService = new EncryptionService(config);
var temperatureDataManager = new TemperatureDataManager(config, encryptionService);

var simulator1 = new IoTSimulator(temperatureDataManager, "iot-device-1", 10, 20, 40, 50);
var simulator2 = new IoTSimulator(temperatureDataManager, "iot-device-2", 15, 25, 35, 40);

var tasks = new List<Task>
{
    simulator1.StartSimulationAsync(),
    simulator2.StartSimulationAsync()
};

try
{
    await Task.WhenAll(tasks);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}