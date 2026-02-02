using MarketPrice.Domain;
using MarketPrice.Domain.Market.DTOs;
using MarketPrice.Services.Interfaces;
using MarketPrice.Services.Workers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPrice.Api.Controllers
{

    //controller to handle market insights requests
    [Route("[controller]")]
    [ApiController]
    public class MarketsController(
        IMarketService marketService,
        ILogger<MarketsController> logger) : ControllerBase
    {
        private readonly IMarketService _service = marketService;
        private readonly ILogger<MarketsController> _logger = logger;

        // Load Market Data
        //[Authorize]
        [HttpGet(ApiRoutes.LOAD_MARKET_DATA)]
        public async Task<ActionResult<List<MarketResponseDto>>> LoadMarketData()
        {
            try
            {
                var result = await _service.GetMarketTrendAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching market insights for all Commodities");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        // Get Market Insight
        //[Authorize]
        [HttpGet(ApiRoutes.GET_MARKET_INSIGHT)]
        public async Task<ActionResult<MarketInsightResponseDto>> GetMarketDetails(Guid commodityId)
        {
            return Ok(await _service.GetMarketInsightAsync(commodityId));
        }

        // Get Price Chart Data
        //[Authorize]
        [HttpGet(ApiRoutes.GET_CHART_DATA)]
        public async Task<ActionResult<List<MarketInsightChartResponseDto>>> GetChartData(Guid commodityId, [FromQuery] string range = "1m")
        {
            try
            {
                var data = await _service.GetPriceChartAsync(commodityId, range);
                if (data == null || data.Count == 0)
                {
                    return NotFound("No chart data found for the specified commodity and range.");
                }
                return Ok(new
                {
                    data,
                    IsRunnning = true,
                    LastRun = MarketAggregationWorker.LastSuccessfulRun,
                    CurrentStatus = MarketAggregationWorker.Status

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching price chart data for CommodityId: {CommodityId} and Range: {Range}", commodityId, range);
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}