using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Position.DTOs
{
    public class PositionListingResponseDto
    {
        public string? UserName { get; set; }
        public decimal Quantity { get; set; }
        public string? CommodityName { get; set; }
        public string? UnitOfMeasure { get; set; }


    }
}
