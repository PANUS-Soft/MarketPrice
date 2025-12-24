using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Domain.Authentication.DTOs;
using MarketPrice.Services.Implementations;
using MarketPrice.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MarketPrice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUsersController : ControllerBase
    {
        readonly ILogger _logger;
        private IRegisterService _registerService;
        private ILoginService _loginService;

        public ApplicationUsersController(
            IRegisterService registerService,
            ILoginService loginService,
            ILogger<ApplicationUsersController> logger)
        {
            _registerService = registerService;
            _loginService = loginService;
            _logger = logger;
        }

        [HttpPost("auth/register")]
        public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterCommand registerCommand)
        {
            if (registerCommand == null)
                return BadRequest();

            var result = await _registerService.RegisterAsync(registerCommand);

            if (result.success)
                return Ok(result);
            else
                return BadRequest("User registration failed");
        }

        [HttpPost("auth/login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginCommand loginCommand)
        {
            if(loginCommand == null)
                return BadRequest();

            var result = await _loginService.LoginAsync(loginCommand);

            if (result.LoginStatus)
                return Ok(result);
            else
                return Unauthorized("Invalid credentials");
        }

        [HttpPost("auth/logout")]
        public async Task<ActionResult<LogoutResponseDto>> Logout([FromBody] LogoutCommand logoutCommand)
        {
            if (logoutCommand == null)
                return BadRequest();

            var result = await _loginService.LogoutAsync(logoutCommand);

            return Ok(result);
        }
    }
}
