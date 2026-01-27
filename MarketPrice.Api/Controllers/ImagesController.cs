using MarketPrice.Data.Models;
using MarketPrice.Domain;
using MarketPrice.Domain.Image.DTOs;
using MarketPrice.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketPrice.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImagesController (IImageService imageService, ILogger<ImagesController> logger) : ControllerBase
    {
        [HttpGet(ApiRoutes.LOAD_IMAGE)]
        public async Task<ActionResult<ImageResponseDto>> GetImage([FromRoute] Guid id)
        {
            if (id == Guid.Empty) return BadRequest("Invalid Commodity Type Id");

            var imageDto = await imageService.GetCommodityTypeImageAsync(id);

            if (imageDto == null || imageDto.ImageData == null)
            {
                logger.LogWarning("No image found for CommodityTypeId/CommodityId: {Id}", id);
                return NotFound($"Image with Id {id} not found");
            }

            return File(imageDto.ImageData, imageDto.ContentType);
        }

    }
}
