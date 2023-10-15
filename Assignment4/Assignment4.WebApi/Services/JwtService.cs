using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Assignment4.WebApi.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Genererar samt returnerar en JWT med den ClaimsIdentity som anges.
        /// Issuer, Audience och SecretKey hämtas ifrån appsettings med hjälp utav _config
        /// </summary>
        /// <param name="claimsIdentity"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        public string Generate(ClaimsIdentity claimsIdentity, DateTime expiresAt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _config.GetSection("TokenValidation").GetValue<string>("Issuer")!,
                Audience = _config.GetSection("TokenValidation").GetValue<string>("Audience")!,
                Subject = claimsIdentity,
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_config.GetSection("TokenValidation").GetValue<string>("SecretKey")!)),
                    SecurityAlgorithms.HmacSha512Signature
                )
            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(securityTokenDescriptor));
        }
    }
}
