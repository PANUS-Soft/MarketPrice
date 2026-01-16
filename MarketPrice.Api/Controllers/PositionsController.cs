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
        [Authorize]
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
        [Authorize]
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
    }
}
