namespace Assignment4.IoTTempSimulator.Models
{
    internal class TemperatureData
    {
        public string DeviceId { get; set; } = null!;
        public double Temperature { get; set; }
        public int Humidity { get; set; }
        public DateTime DateSent { get; set; }
    }
}
