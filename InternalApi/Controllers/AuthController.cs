using InternalApi.Service;
using Microsoft.AspNetCore.Mvc;

namespace InternalApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var token = await _authService.Login(loginRequest.userName, loginRequest.password);
            if(string.IsNullOrEmpty(token))
            {
                return NotFound();
            }
            else
            {
                return Ok(token);
            }
        }
    }


    public class LoginRequest
    {
        public string userName { get; set; }
        public string password { get; set; }
    }


}
