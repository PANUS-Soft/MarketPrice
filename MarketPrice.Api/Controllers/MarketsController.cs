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
        [HttpGet(ApiRoutes.MARKET_INSIGHTS + "/{commodityTypeId:guid}")]
        public async Task<ActionResult<List<MarketInsightResponseDto>>> GetByCommodityType(Guid? commodityTypeId)
        {
           

            try
            {
                var command = new MarketInsightCommand { CommodityTypeId = commodityTypeId ?? Guid.Empty };
                var result = await _marketService.GetMarketTrendAsync(command);

                if (result == null || result.Count == 0)
                    return NotFound("No active market positions found for this commodity type.");

                return Ok(result ?? new List<MarketInsightResponseDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching market depth for CommodityTypeId: {CommodityTypeId}", commodityTypeId);
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

    }
}