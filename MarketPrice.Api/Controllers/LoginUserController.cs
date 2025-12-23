using MarketPrice.Domain.Authentication.Commands;
using MarketPrice.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketPrice.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class LoginUserController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginUserController(ILoginService loginService)
        {
            _loginService = loginService;
            
        }

        [HttpPost("login")]

        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _loginService.LoginAsync(command);

            if (!result.LoginStatus)
            {
                return Unauthorized(new { message = "Invalide email or password" });
            }

            return Ok(result);
        }


        [HttpPost]

        public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
        {
            var result = await _loginService.LogoutAsync(command);
            return Ok(result);
        }

    }
}
