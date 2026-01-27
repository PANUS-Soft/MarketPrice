using MarketPrice.Domain;
using MarketPrice.Domain.Market.DTOs;
using MarketPrice.Services.Interfaces;
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
       
        // GET MARKET INSIGHTS
        [Authorize]
        [HttpGet(ApiRoutes.LOAD_MARKET_DATA)]
        public async Task<ActionResult<List<MarketResponseDto>>> LoadMarketData()
        {
            try
            {
                var result = await marketService.GetMarketTrendAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching market insights for all Commodities");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

    }
}