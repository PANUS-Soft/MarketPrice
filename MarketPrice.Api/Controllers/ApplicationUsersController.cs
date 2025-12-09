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
        private IMarketPriceAuthenticationService _marketPriceauthenticationService;

        public ApplicationUsersController(
            IMarketPriceAuthenticationService marketPriceAuthenticationService,
            ILogger<ApplicationUsersController> logger) {

            _marketPriceauthenticationService = marketPriceAuthenticationService;
            _logger  = logger;
        }

        
        // here was to create a new User.
        [HttpPost]
        public async Task<ActionResult<LoginResponseDto>> AuthenticateUser(LoginCommand command)
        {
            var authenticated = _marketPriceauthenticationService.Authenticate(command.Username, command.Password);

            LoginResponseDto response;
            if (authenticated)
            {
                response = new LoginResponseDto
                {
                    IsAuthenticated = authenticated,
                    FirstName = "Gerald",
                    FamilyName = "Nupa",
                    RememberMe = command.RememberMe,
                    Username = command.Username,
                };

            }
            else
            {
                response = new LoginResponseDto
                {
                    IsAuthenticated = false,
                    RememberMe = command.RememberMe,
                    Username = command.Username,
                };
            }

            return await Task.FromResult(response);
        }





    }
}
