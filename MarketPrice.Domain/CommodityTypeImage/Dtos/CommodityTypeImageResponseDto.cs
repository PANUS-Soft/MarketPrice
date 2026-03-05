using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.CommodityTypeImage.DTOs
{
    public class CommodityTypeImageResponseDto
    {
        public required byte[] ImageData { get; set; }
        public required string ContentType { get; set; }
    }
}
