using Assignment4.IoTTempSimulator.Services;
using Assignment4.IoTTempSimulator.Simulators;
using Microsoft.Extensions.Configuration;

// Lägger till config.json i builder contexten för att kunna hämta data därifrån.
var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("config.json", optional: false, reloadOnChange: true);

IConfiguration config = builder.Build();

var authService = new AuthService();
var encryptionService = new EncryptionService(config);
var temperatureDataManager = new TemperatureDataManager(config, encryptionService);

// Här sätts 3 olika instanser av IoTSimulator upp med lite olika värden, alla tre kommer att lyckas bli auktoriserade men endast 1 och 2 är autentiserade att skicka data.
#region Simulators Setup
var simulator1 = new IoTSimulator(authService,
                                  temperatureDataManager,
                                  "iot-device-1",
                                  "sensor",
                                  10,
                                  15,
                                  45,
                                  50);

var simulator2 = new IoTSimulator(authService,
                                  temperatureDataManager,
                                  "iot-device-2",
                                  "sensor",
                                  20,
                                  25,
                                  35,
                                  40);

var simulator3 = new IoTSimulator(authService,
                                  temperatureDataManager,
                                  "iot-device-3",
                                  "user",
                                  20,
                                  25,
                                  35,
                                  40);
#endregion

// Skapar en lista av Tasks för att köra alla simulatorer samtidigt
var tasks = new List<Task>
{
    simulator1.StartSimulationAsync(),
    simulator2.StartSimulationAsync(),
    simulator3.StartSimulationAsync()
};

// Kör alla tasks och fångar eventuella fel
try
{
    await Task.WhenAll(tasks);
}
catch (Exception ex)
{
    Console.WriteLine("Error: " + ex.Message);
    Console.ReadLine();
}