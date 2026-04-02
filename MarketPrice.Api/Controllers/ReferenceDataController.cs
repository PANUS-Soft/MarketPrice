using MarketPrice.Domain;
using MarketPrice.Domain.Reference.DTOs;
using MarketPrice.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketPrice.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReferenceDataController(
        IReferenceDataService referenceDataService,
        ILogger<ReferenceDataController> logger) : ControllerBase
    {
        private readonly ILogger _logger = logger;

        [Authorize]
        [HttpGet(ApiRoutes.REF_REGION)]
        public async Task<ActionResult<RegionDto>> GetRegions()
        {
            var result = await referenceDataService.GetRegionAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpGet(ApiRoutes.REF_COMMODITY_TYPE)]
        public async Task<ActionResult<CommodityTypeDto>> GetCommodityTypes()
        {
            var result = await referenceDataService.GetCommodityTypeAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpGet(ApiRoutes.REF_COMMODITY + "/{id}")]
        public async Task<ActionResult<CommodityDto>> GetCommoditiesByTypeId([FromRoute] Guid id)
        {
            var result = await referenceDataService.GetCommodityByIdAsync(id);
            return Ok(result);
        }

        [Authorize]
        [HttpGet(ApiRoutes.REF_COMMODITY)]
        public async Task<ActionResult<CommodityDto>> GetAllCommodities()
        {
            var result = await referenceDataService.GetAllCommoditiesAsync();
            return Ok(result);
        }
    }
}