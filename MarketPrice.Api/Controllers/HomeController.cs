using Microsoft.AspNetCore.Mvc;
using MarketPrice.Services.Interfaces;
using MarketPrice.Domain.Home.DTOs;
using MarketPrice.Domain;
using Microsoft.AspNetCore.Authorization;

namespace MarketPrice.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController(IHomeService homeService, ILogger<HomeController> logger) : ControllerBase
    {
        private readonly IHomeService _homeService = homeService;
        private readonly ILogger<HomeController> _logger = logger;

        /// <summary>
        /// Loads home market data for all commodity types
        /// </summary>

        [Authorize]
        [HttpGet(ApiRoutes.LOAD_HOME_DATA)]
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
                _logger.LogInformation(ex, $"An error occurred while loading home market data. {ex.Message}");
                //_logger.LogError(ex, $"An error occurred while loading home market data. {ex.Message}");

                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while retrieving market data.",
                    details = "Please try again later."
                });
            }
        }
    }
}