using System.ComponentModel.DataAnnotations;

namespace Assignment4.WebApi.DTOs
{
    public class TemperatureDataRequest
    {
        [Required]
        [MinLength(2), MaxLength(20)]
        public string DeviceId { get; set; } = null!;

        [Required]
        [Range(-40.0, 120.0)]
        public double Temperature { get; set; }

        [Required]
        [Range(0, 100)]
        public int Humidity { get; set; }

        [Required]
        public DateTime DateSent { get; set; }
    }
}
