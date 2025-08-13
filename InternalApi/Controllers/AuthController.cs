using InternalApi.Models;
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
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var token = await _authService.Login(loginModel.userName, loginModel.password);
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

}
