using System.ComponentModel.DataAnnotations;

namespace Assignment4.WebApi.DTOs
{
    public class LoginRequest
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}
