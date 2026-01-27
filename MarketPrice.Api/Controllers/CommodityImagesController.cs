using MarketPrice.Domain;
using MarketPrice.Domain.Image.DTOs;
using MarketPrice.Services.Implementations;
using MarketPrice.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MarketPrice.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CommodityImagesController (IImageService imageService, ILogger<CommodityImagesController> logger) : ControllerBase
    {
        [HttpGet(ApiRoutes.LOAD_IMAGE)]
        public async Task<ActionResult<ImageResponseDto>> GetImage([FromRoute] Guid id)
        {
            if (id == Guid.Empty) return BadRequest("Invalid Commodity Id");

            var imageDto = await imageService.GetCommodityImageAsync(id);

            if (imageDto == null || imageDto.ImageData == null)
            {
                logger.LogWarning("No image found for CommodityId: {Id}", id);
                return NotFound($"Image with Id {id} not found");
            }

            return File(imageDto.ImageData, imageDto.ContentType);
        }
    }
}
