using MarketPrice.Domain;
using MarketPrice.Domain.ChangePassword.Command;
using MarketPrice.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketPrice.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ChangePasswordController : ControllerBase
    {

        private readonly IUserSecurityService _userSecurityService;

        public ChangePasswordController(IUserSecurityService userSecurityService)
        {
            _userSecurityService = userSecurityService;
        }

        [HttpPost (ApiRoutes.changePwd)]
        public async Task<IActionResult> ChangePassword(
            Guid userId,
            ChangePasswordCommand command)
        {
            var result = await _userSecurityService.ChangePasswordAsync(userId, command);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

    }
}
