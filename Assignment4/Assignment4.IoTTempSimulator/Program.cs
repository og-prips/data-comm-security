using Assignment4.IoTTempSimulator.Services;
using Assignment4.IoTTempSimulator.Simulators;

var signalRManager = new TemperatureDataManager();
var iotSimulator = new IoTSimulator(signalRManager);

try
{
    await iotSimulator.StartSimulationAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}