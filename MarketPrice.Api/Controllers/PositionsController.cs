using MarketPrice.Domain.Position.Commands;
using MarketPrice.Domain.Position.DTOs;
using MarketPrice.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarketPrice.Domain;
using Microsoft.AspNetCore.Authorization;

namespace MarketPrice.Api.Controllers
{
    //[Route("[controller]")]
    //[ApiController]

    [ApiController]
    [Route("[controller]")]
    public class PositionsController : ControllerBase
    {
        private readonly IPositionService _positionService;

        public PositionsController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        // CREATE BID
        //[Authorize]
        [HttpPost(ApiRoutes.BID_CREATE)]
        public async Task<ActionResult<PositionResponseDto>> CreateBid(
            [FromBody] PositionCommand command)
        {
            var response = await _positionService.ProcessPositionAsync(
                command,
                isOffer: false
            );

            return CreatedAtAction(
                nameof(GetPosition),
                new { id = response.PositionId },
                response
            );
        }

        // CREATE OFFER
        //[Authorize]
        [HttpPost(ApiRoutes.OFFER_CREATE)]
        public async Task<ActionResult<PositionResponseDto>> CreateOffer(
            [FromBody] PositionCommand command)
        {
            var response = await _positionService.ProcessPositionAsync(
                command,
                isOffer: true
            );

            return CreatedAtAction(
                nameof(GetPosition),
                new { id = response.PositionId },
                response
            );
        }

        // -----------------------------
        // GET POSITION (testing)
        // -----------------------------
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPosition(Guid id)
        {
            // Optional: add read service later
            return Ok(new { PositionId = id });
        }

        // List of Positions for a given price.
        //[Authorize]
        [HttpPost(ApiRoutes.POSITION_BYPRICE)]
        public async Task<ActionResult<List<PositionListingResponseDto>>> GetPositionsForPrice(
            [FromBody] PositionListingCommand command)
        {
            if (command == null)
                return BadRequest("Request body is required.");

            if (command.CommodityTypeId == Guid.Empty)
                return BadRequest("CommodityTypeId is required.");

            if (command.PositionTypeId <= 0)
                return BadRequest("PositionTypeId is required and must be greater than zero.");

            if (command.UnitPrice == null)
                return BadRequest("UnitPrice is required.");

            var results = await _positionService.GetPositionsForPriceAsync(command);

            return Ok(results);
        }
    }
}
