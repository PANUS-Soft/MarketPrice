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
            Console.WriteLine($"{loginCommand.EmailAddress} is attempting to login into the platform ...");

            var result = await loginService.LoginAsync(loginCommand);

            if (result.Success)
            {
                Console.WriteLine($"User successfully logged in with access token expiration in {result.ExpiryDate} minutes");
                return Ok(result);
            }
            else
                return Unauthorized(result.Status);

            Console.WriteLine($"{result.Errors}");
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
            Console.WriteLine("Tokens refresh request from a client ...");
            Console.WriteLine($"Access token: {refreshTokenCommand.AccessToken}");
            Console.WriteLine($"Refresh token: {refreshTokenCommand.RefreshToken}");
            var result = await refreshTokenService.RefreshTokenAsync(refreshTokenCommand);
            Console.WriteLine("Tokens refresh process completed successfully with new information as ... ");
            Console.WriteLine($"New access token: {result.AccessToken}");
            Console.WriteLine($"New access token: {result.RefreshToken}");
            Console.WriteLine($"New access token: {result.ExpiryDate}");
            return Ok(result);
        }

        [Authorize]
        [HttpGet("auth/ping")]
        public IActionResult Ping()
        {
            return Ok("Alive 😁😁😁");
        }
    }
}
