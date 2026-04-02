using MarketPrice.Domain;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Domain.Position.DTOs;
using MarketPrice.Services.Implementations;
using MarketPrice.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MarketPrice.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PositionsController(IPositionService positionService) : ControllerBase
    {
        // CREATE BID
        // [Authorize]
        [HttpPost(ApiRoutes.BID_CREATE)]
        public async Task<ActionResult<PositionResponseDto>> CreateBid([FromBody] PositionCommand command)
        {
            var response = await positionService.ProcessPositionAsync(command, isOffer: false);

            return CreatedAtAction(nameof(GetPosition), new { id = response.PositionId }, response);
        }

        // CREATE OFFER
        // [Authorize]
        [HttpPost(ApiRoutes.OFFER_CREATE)]
        public async Task<ActionResult<PositionResponseDto>> CreateOffer([FromBody] PositionCommand command)
        {
            var response = await positionService.ProcessPositionAsync(command, isOffer: true);

            return CreatedAtAction(nameof(GetPosition), new { id = response.PositionId }, response);
        }

        // GET POSITION (testing)
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPosition(Guid id)
        {
            return Ok(new { PositionId = id });
        }

        // List of Positions for a specific commodity type, position type, and unit price
        [Authorize]
        [HttpPost(ApiRoutes.POSITION_BY_PRICE)]
        public async Task<ActionResult<PositionListingResponseDto>> GetPositionsForPrice([FromBody] PositionListingCommand command)
        {
            var results = await positionService.GetPositionListingsAsync(command);
            if (results.Success)
            {
                return Ok(results);
            }

            return BadRequest(results);
        }

        // Get Position Detail
        [HttpGet(ApiRoutes.POSITION_DETAIL + "/{id}")]
        public async Task<ActionResult<PositionDetailResponseDto>> GetPositionDetail(Guid id)
        {
            var result = await positionService.GetPositionDetailAsync(id);

            return Ok(result);
        }
    }
}
