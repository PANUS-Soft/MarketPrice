using MarketPrice.Domain;
using MarketPrice.Domain.UserProfile.Command;
using MarketPrice.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MarketPrice.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]

    public class ProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public ProfileController( IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
            
        }
        private Guid GetUserId()
        {

            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet(ApiRoutes.Get_UserProfile)]
        public async Task<IActionResult> GetMyProfile()
        {
            var UserId = GetUserId();
            var profile = await _userProfileService.GetUserProfile(UserId);
            return Ok(profile);
        }

        [HttpPut(ApiRoutes.Edit_UserProfile)]

        public async Task<IActionResult> EditProfile([FromBody] EditUserProfileCommand command)
        {
            var UserId = GetUserId();
            await _userProfileService.UpdateUserProfile(UserId, command);
            return NoContent();

        }


    }
}
