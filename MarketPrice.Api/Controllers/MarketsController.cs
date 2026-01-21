using MarketPrice.Domain;
using MarketPrice.Domain.Market.Commands;
using MarketPrice.Domain.Market.Dtos;
using MarketPrice.Services.Implementations;
using MarketPrice.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace MarketPrice.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MarketsController(
        IMarketService marketService,
        ILogger<MarketsController> logger) : ControllerBase
    {
        private readonly IMarketService _marketService = marketService;
        private readonly ILogger _logger = logger;

        /// <summary>
        /// Retrieves market depth (bids and offers) for a given commodity type.
        /// POST /Markets/depth
        /// Body: MarketDepthCommand { CommodityTypeId: Guid }
        /// </summary>

        [Authorize]
        [HttpPost(ApiRoutes.MARKET_INSIGHTS)]
        public async Task<ActionResult<MarketDepthResponseDto>> GetInsight([FromBody] MarketDepthCommand command)
        {
            if (command == null || command.CommodityTypeId == Guid.Empty)
                return BadRequest("CommodityTypeId is required.");

            try
            {
                var result = await _marketService.GetMarketTrendAsync(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching market depth for CommodityTypeId: {CommodityTypeId}", command?.CommodityTypeId);
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}