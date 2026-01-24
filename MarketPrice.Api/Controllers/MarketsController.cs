using MarketPrice.Data.Models;
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

    //controller to handle market insights requests
    [Route("[controller]")]
    [ApiController]
    public class MarketsController(
        IMarketService marketService,
        ILogger<MarketsController> logger) : ControllerBase
    {
        private readonly IMarketService _marketService = marketService;
        private readonly ILogger _logger = logger; // will be use fro logging errors and filtering

        // GET MARKET INSIGHTS
        //[Authorize]
        [HttpGet(ApiRoutes.MARKET_INSIGHTS)]
        [ProducesResponseType(typeof(List<MarketInsightResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<MarketInsightResponseDto>>> GetByCommodityType()
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

    }
}