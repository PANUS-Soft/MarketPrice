using MarketPrice.Domain;
using MarketPrice.Domain.Market.DTOs;
using MarketPrice.Services.Interfaces;
using MarketPrice.Services.Workers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPrice.Api.Controllers
{

    // Controller to handle market insights requests
    [Route("[controller]")]
    [ApiController]
    public class MarketsController(
        IMarketService marketService,
        ILogger<MarketsController> logger) : ControllerBase
    {
        private readonly IMarketService _marketService = marketService;
        private readonly ILogger<MarketsController> _logger = logger;

        // Load Market Data
        [HttpGet(ApiRoutes.LOAD_MARKET_DATA)]
        public async Task<ActionResult<List<MarketResponseDto>>> LoadMarketData()
        {
            try
            {
                var result = await _marketService.GetMarketTrendAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching market insights for all Commodities");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        // Get Market Insight
        [Authorize]
        [HttpGet(ApiRoutes.GET_MARKET_INSIGHT + "/{commodityId}")]
        public async Task<ActionResult<MarketInsightResponseDto>> GetMarketDetails(Guid commodityId)
        {
            return Ok(await _marketService.GetMarketInsightAsync(commodityId));
        }

        // Get Price Chart Data
        [Authorize]
        [HttpGet(ApiRoutes.GET_CHART_DATA)]
        public async Task<ActionResult> GetChartData( [FromQuery] Guid commodityId, [FromQuery] string range = "1D")
        {
            try
            {
                if (commodityId == Guid.Empty)
                    return BadRequest("Invalid commodityId");

                range = range.ToUpper();

                var validRanges = new[] { "1M", "1D", "1W","1Y" };

                if (!validRanges.Contains(range))
                    //&& range != "1m"
                    return BadRequest("Invalid range");

                var data = await _marketService.GetPriceChartAsync(commodityId, range);

                if (data == null || data.Count == 0)
                {
                    return NotFound("No chart data found.");
                }

                return Ok(new
                {
                    data,
                    IsRunning = true,
                    LastRun = MarketAggregationWorker.LastSuccessfulRun,
                    CurrentStatus = MarketAggregationWorker.Status
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching chart data for CommodityId: {CommodityId}, Range: {Range}",
                    commodityId, range);

                return StatusCode(500, "Internal server error");
            }
        }
    }
}