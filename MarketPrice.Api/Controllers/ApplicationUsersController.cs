using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Domain.Authentication.DTOs;
using MarketPrice.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketPrice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUsersController(
        IRegisterService registerService,
        ILoginService loginService,
        ILogoutService logoutService,
        ILogger<ApplicationUsersController> logger) : ControllerBase
    {
        private readonly ILogger _logger = logger;
        private readonly IRegisterService _registerService = registerService;
        private readonly ILoginService _loginService = loginService;
        private readonly ILogoutService _logoutService = logoutService;

        [HttpPost("auth/register")]
        public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterCommand registerCommand)
        {
            if (registerCommand == null)
                return BadRequest("Invalid request data");

            var result = await _registerService.RegisterAsync(registerCommand);

            if (result.Success)
                return Ok(result);
            else
                return Conflict(result.CreationStatus);
        }

        [HttpPost("auth/login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginCommand loginCommand)
        {
            if (loginCommand == null)
                return BadRequest("Invalid request data");

            var result = await _loginService.LoginAsync(loginCommand);

            if (result.Success)
                return Ok(result);
            else
                return Unauthorized(result.LoginStatus);
        }

        [HttpPost("auth/logout")]
        public async Task<ActionResult<LogoutResponseDto>> Logout([FromBody] LogoutCommand logoutCommand)
        {
            if (logoutCommand == null)
                return BadRequest("Invalid request data");

            var result = await _logoutService.LogoutAsync(logoutCommand);

            return Ok(result);
        }
    }
}
