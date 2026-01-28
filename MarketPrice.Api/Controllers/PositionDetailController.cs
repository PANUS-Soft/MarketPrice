using MarketPrice.Domain;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketPrice.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PositionDetailController: ControllerBase
    {
        private readonly IPositionDetailService _positionDetailService;

        public PositionDetailController (IPositionDetailService positionDetailService)
        {
            _positionDetailService = positionDetailService;
        }

        [HttpPost(ApiRoutes.Cart_Position)]
        public async Task<IActionResult> GetPositionDetail([FromBody] PositionDetailCommand command)
        {
            var result = await _positionDetailService.GetPositionDetailAsync(command);
            return Ok(result);
        }
    }
}
