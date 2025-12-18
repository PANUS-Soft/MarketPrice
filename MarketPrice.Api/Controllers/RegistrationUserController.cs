using MarketPrice.Domain.Authentication.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore.Internal;
using MarketPrice.Services.Interfaces;

namespace MarketPrice.Api.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationUserController : ControllerBase 
    {
        private readonly IRegistrationService _registeredServices;

        public RegistrationUserController(IRegistrationService registeredServices)
        {
            _registeredServices = registeredServices;
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> Register([FromBody] RegistrationCommand command)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _registeredServices.RegisterAsync(command);

            if(!result.success)
            {
                return BadRequest(ModelState);
            }
            return Ok(result);
        }

    }
}
