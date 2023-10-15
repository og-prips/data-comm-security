using Assignment4.IoTTempSimulator.Models;
using System.Net.Http.Json;

namespace Assignment4.IoTTempSimulator.Services
{
    internal class AuthService
    {
        /// <summary>
        /// Skickar en förfrågan till APIet med en angiven identitet, returnerar en JWT om lyckad request, annars en tom sträng
        /// </summary>
        /// <param name="identityName"></param>
        /// <returns></returns>
        public async Task<string> Login(string identityName)
        {
            var request = new Login() { Name = identityName };

            var client = new HttpClient();
            var result = await client.PostAsJsonAsync("https://localhost:7205/api/Authentication/Login", request);

            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsStringAsync();
            }

            return string.Empty;
        }
    }
}
