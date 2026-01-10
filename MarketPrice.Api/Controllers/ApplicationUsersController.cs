using MarketPrice.Domain;
using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Domain.Authentication.DTOs;
using MarketPrice.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPrice.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApplicationUsersController(
        IRegisterService registerService,
        ILoginService loginService,
        ILogoutService logoutService,
        IRefreshTokenService refreshTokenService,
        ILogger<ApplicationUsersController> logger) : ControllerBase
    {
        private readonly ILogger _logger = logger;

        [HttpPost(ApiRoutes.AUTH_REGISTER)]
        public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterCommand registerCommand)
        {
            var result = await registerService.RegisterAsync(registerCommand);

            if (result.Success)
                return Ok(result);
            else
                return Conflict(result.Status);
        }

        [HttpPost(ApiRoutes.AUTH_LOGIN)]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginCommand loginCommand)
        {
            var result = await loginService.LoginAsync(loginCommand);

            if (result.Success)
            {
                return Ok(result);
            }
            else
                return Unauthorized(result.Status);
        }

        [HttpPost(ApiRoutes.AUTH_LOGOUT)]
        public async Task<ActionResult<LogoutResponseDto>> Logout([FromBody] LogoutCommand logoutCommand)
        {
            var result = await logoutService.LogoutAsync(logoutCommand);

            return Ok(result);
        }

        [HttpPost(ApiRoutes.AUTH_REFRESH_TOKEN)]
        public async Task<ActionResult<RefreshTokenResponseDto>> RefreshToken([FromBody] RefreshTokenCommand refreshTokenCommand)
        {
            var result = await refreshTokenService.RefreshTokenAsync(refreshTokenCommand);
            return Ok(result);
        }

        [Authorize]
        [HttpGet(ApiRoutes.AUTH_PING)]
        public IActionResult Ping()
        {
            return Ok("Alive 😁😁😁");
        }
    }
}
