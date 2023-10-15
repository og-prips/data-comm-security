using Assignment4.WebApi.DTOs;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Assignment4.WebApi.Services
{
    public class AuthService
    {
        // En hårdkodad lista med roller
        private List<IdentityRole> _roles = new List<IdentityRole>()
        {
            new IdentityRole("sensor"),
            new IdentityRole("user")
        };

        private JwtService _jwtService;

        public AuthService(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        /// <summary>
        /// Tar emot en LoginRequest och kollar ifall identiteten i request finns i listan _roles.
        /// Returnerar en tom string om rollen inte finns.
        /// Ifall rollen finns skapas en ny claim med rollenm som används för att generera giltig JWT som i sin tur returneras.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        public string Login(LoginRequest request)
        {
            string role = _roles.FirstOrDefault(x => x.Name == request.Name)?.Name;

            if (string.IsNullOrEmpty(role))
            {
                return string.Empty;
            }

            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, role)
            });

            return _jwtService.Generate(claimsIdentity, DateTime.Now.AddHours(1));
        }
    }
}
