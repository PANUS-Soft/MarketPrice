using MarketPrice.Domain;
using MarketPrice.Domain.Activity.DTOs;
using MarketPrice.Domain.Authentication;
using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Domain.Authentication.DTOs;
using MarketPrice.Domain.Profile.Commands;
using MarketPrice.Domain.Profile.DTOs;
using MarketPrice.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MarketPrice.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApplicationUsersController(
        IRegisterService registerService,
        ILoginService loginService,
        ILogoutService logoutService,
        IRefreshTokenService refreshTokenService,
        IProfileService profileService,
        IChangePasswordService changePasswordService,
        IPositionService positionService,
        ILogger<ApplicationUsersController> logger) : ControllerBase
    {
        private readonly ILogger _logger = logger;

        [HttpPost(ApiRoutes.AUTH_REGISTER)]
        public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterCommand command)
        {
            var result = await registerService.RegisterAsync(command);

            if (result.Success)
            {
                return Ok(result);
            }

            return Conflict(result.Status);
        }

        [HttpPost(ApiRoutes.AUTH_LOGIN)]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginCommand command)
        {
            var result = await loginService.LoginAsync(command);

            if (result.Success)
            {
                return Ok(result);
            }
            
            return Unauthorized(result.Status);
        }

        [HttpPost(ApiRoutes.AUTH_LOGOUT)]
        public async Task<ActionResult<LogoutResponseDto>> Logout([FromBody] LogoutCommand command)
        {
            var result = await logoutService.LogoutAsync(command);

            return Ok(result);
        }

        [HttpPost(ApiRoutes.AUTH_REFRESH_TOKEN)]
        public async Task<ActionResult<AuthenticationResponseDto>> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            var result = await refreshTokenService.RefreshTokenAsync(command);
            return Ok(result);
        }

        //[Authorize]
        [HttpGet(ApiRoutes.AUTH_PING)]
        public IActionResult Ping()
        {
            return Ok("Alive 😁😁😁");
        }

        [Authorize]
        [HttpGet(ApiRoutes.GET_USER_PROFILE + "/{id}")]
        public async Task<ActionResult<UserProfileResponseDto>> GetUserProfile(Guid id)
        {
            var result = await profileService.GetUserProfileAsync(id);

            return Ok(result);
        }

        [Authorize]
        [HttpPatch(ApiRoutes.UPDATE_USER_PROFILE)]
        public async Task<ActionResult<UpdateUserProfileResponseDto>> UpdateUserProfile([FromBody] UpdateUserProfileCommand command)
        {
            var result = await profileService.UpdateUserProfileAsync(command);

            if (result.Status)
            {
                return Ok(result);
            }

            return Conflict(result);
        }

        [Authorize]
        [HttpPost(ApiRoutes.CHANGE_PASSWORD)]   
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            var result = await changePasswordService.ChangePasswordAsync(command);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        //[Authorize]
        [HttpGet(ApiRoutes.GET_USER_ACTIVITY + "/{id}")]
        public async Task<ActionResult<ActivityGroupDto>> GetActivity(Guid id)
        {
            var result = await positionService.GetActivityAsync(id);
            return Ok(result);
        }

    }
}
