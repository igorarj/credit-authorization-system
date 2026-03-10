using CreditAuthorizationSystem.Auth.Api.Contracts;
using CreditAuthorizationSystem.Auth.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CreditAuthorizationSystem.Auth.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IRegisterUserService _registerService;
        private readonly ILoginService _loginService;

        public AuthController(
            IRegisterUserService registerService,
            ILoginService loginService)
        {
            _registerService = registerService;
            _loginService = loginService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            await _registerService.RegisterAsync(request.Email, request.Password);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var token = await _loginService.LoginAsync(request.Email, request.Password);

            if (token == null)
                return Unauthorized();

            return Ok(new { token });
        }
    }
}
