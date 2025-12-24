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
        private readonly IRegisterService _registeredServices;

        public RegistrationUserController(IRegisterService registeredServices)
        {
            _registeredServices = registeredServices;
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
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
