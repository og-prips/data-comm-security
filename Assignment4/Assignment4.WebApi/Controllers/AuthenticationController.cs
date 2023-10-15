using Assignment4.WebApi.DTOs;
using Assignment4.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Assignment4.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthenticationController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Validerar att requesten är giltig och returnerar isåfall en JWT om Login metoden inte returnerar en null/tom sträng.
        /// Ifall requesten inte är valid returneras 401 (Unauthorized)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("Login")]
        [HttpPost]
        public IActionResult Login(LoginRequest request)
        {
            if (ModelState.IsValid)
            {
                var token = _authService.Login(request);
                if (!string.IsNullOrEmpty(token))
                {
                    return Ok(token);
                }
            }

            return Unauthorized("Incorrect login");
        }
    }
}
