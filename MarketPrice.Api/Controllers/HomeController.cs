using Microsoft.AspNetCore.Mvc;
using MarketPrice.Services.Interfaces;
using MarketPrice.Domain.Home.Dtos;
using MarketPrice.Domain;

namespace MarketPrice.Api.Controllers
{
    [ApiController]
    [Route(ApiRoutes.HOME_DATA)]
    public class HomeController(IHomeService homeService, ILogger<HomeController> logger) : ControllerBase
    {
        private readonly IHomeService _homeService = homeService;
        private readonly ILogger<HomeController> _logger = logger; 
          
        /// <summary>
        /// Loads home market data for all commodity types
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<LoadHomeResponseDto>>> LoadHome()
        {
            try
            {
                var data = await _homeService.LoadHomeAsync();

                if (data == null || data.Count == 0)
                {
                    _logger.LogWarning("LoadHomeAsync returned no data.");
                    return Ok(new List<LoadHomeResponseDto>());
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading home market data.");

                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while retrieving market data.",
                    details = "Please try again later."
                });
            }
        }
    }
}