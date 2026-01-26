using System.Diagnostics;
using MarketPrice.Domain;
using MarketPrice.Domain.Authentication;
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
            _logger.LogInformation("Registration attempt from a user");
            var result = await registerService.RegisterAsync(registerCommand);

            if (result.Success)
            {
                _logger.LogInformation("User registered successfully with the following information ...");
                _logger.LogInformation($"Access token: {result.AccessToken}, Expiry date: {result.ExpiryDate}");
                return Ok(result);
            }
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
            
            return Unauthorized(result.Status);
        }

        [HttpPost(ApiRoutes.AUTH_LOGOUT)]
        public async Task<ActionResult<LogoutResponseDto>> Logout([FromBody] LogoutCommand logoutCommand)
        {
            var result = await logoutService.LogoutAsync(logoutCommand);

            return Ok(result);
        }

        [HttpPost(ApiRoutes.AUTH_REFRESH_TOKEN)]
        public async Task<ActionResult<AuthenticationResponseDto>> RefreshToken([FromBody] RefreshTokenCommand refreshTokenCommand)
        {
            _logger.LogInformation($"Refresh token attempt with the following details ... Refresh token: {refreshTokenCommand.RefreshToken}, UserId: {refreshTokenCommand.UserId}");
            var result = await refreshTokenService.RefreshTokenAsync(refreshTokenCommand);
            _logger.LogInformation($"Success ... New Access token: {result.AccessToken}, Expiry date: {result.ExpiryDate}");
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
