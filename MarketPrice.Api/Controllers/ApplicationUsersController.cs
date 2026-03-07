using MarketPrice.Data.Models;
using MarketPrice.Domain;
using MarketPrice.Domain.Authentication;
using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Domain.Authentication.DTOs;
using MarketPrice.Domain.Profile.DTOs;
using MarketPrice.Domain.UserProfile.Command;
using MarketPrice.Services.Implementations;
using MarketPrice.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace MarketPrice.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApplicationUsersController(
        IRegisterService registerService,
        ILoginService loginService,
        ILogoutService logoutService,
        IRefreshTokenService refreshTokenService,
         IUserProfileService _userProfileService,
       IUserProfileService userProfileService,

        ILogger<ApplicationUsersController> logger) : ControllerBase
    {
        private readonly ILogger _logger = logger;

        [HttpPost(ApiRoutes.AUTH_REGISTER)]
        public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterCommand registerCommand)
        {
            //_logger.LogInformation("Registration attempt from a user");
            var result = await registerService.RegisterAsync(registerCommand);

            if (result.Success)
            {
                //_logger.LogInformation("User registered successfully with the following information ...");
                //_logger.LogInformation($"Access token: {result.AccessToken}, Expiry date: {result.ExpiryDate}");
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
            //_logger.LogInformation($"Refresh token attempt with the following details ... Refresh token: {refreshTokenCommand.RefreshToken}, UserId: {refreshTokenCommand.UserId}");
            var result = await refreshTokenService.RefreshTokenAsync(refreshTokenCommand);
            //_logger.LogInformation($"Success ... New Access token: {result.AccessToken}, Expiry date: {result.ExpiryDate}");
            return Ok(result);
        }

        [Authorize]
        [HttpGet(ApiRoutes.AUTH_PING)]
        public IActionResult Ping()
        {
            return Ok("Alive 😁😁😁");
        }

        [Authorize]
        private Guid GetUserId()
        {

            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet(ApiRoutes.Get_UserProfile)]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = GetUserId();
            var profile = await _userProfileService.GetUserProfile(userId);
            return Ok(profile);
        }

        [HttpPut(ApiRoutes.Edit_UserProfile)]

        public async Task<IActionResult> EditProfile([FromBody] EditUserProfileCommand command)
        {
            var userId = GetUserId();
            await _userProfileService.UpdateUserProfile(userId, command);
            return NoContent();
        }
        
    }
}
