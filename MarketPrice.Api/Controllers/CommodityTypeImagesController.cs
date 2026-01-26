using MarketPrice.Domain;
using MarketPrice.Domain.CommodityTypeImage.Commands;
using MarketPrice.Domain.CommodityTypeImage.Dtos;
using MarketPrice.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketPrice.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CommodityTypeImagesController(ICommodityTypeImageService commodityTypeImageService) : Controller
    {
        private readonly ICommodityTypeImageService _commodiTypeImageService = commodityTypeImageService;

        /// <summary>
        /// Returns the image for a given commodity type.
        /// </summary>
        /// <returns></returns>

        [HttpGet(ApiRoutes.IMAGE_DATA)]
        public async Task<ActionResult<CommodityTypeImageResponseDto>> GetCommodityTypeImage([FromRoute]Guid commodityTypeId)
        {
            if (commodityTypeId == Guid.Empty)
            {
                return BadRequest("Invalid Commodity Type ID.");
            }

            var command = new CommodityTypeImageCommand
            {
                CommodityTypeId = commodityTypeId
            };

            var imageDto = await _commodiTypeImageService.GetCommodityTypeImageAsync(command);
            if (imageDto == null)
            {
                return NotFound();
            }

            return File(imageDto.ImageData, imageDto.ContentType);
        }

    }
}
