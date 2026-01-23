using MarketPrice.Domain.Authentication.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Position.DTOs
{
    public class PositionListingPageResponseDto:BaseResponseDto
    {
        public string? CommodityTypeName { get; set; } 
        public string? PositionTypeName { get; set; }
        public decimal UnitPrice { get; set; }
        public List<string> CommodityNames { get; set; } = new();
        public List<PositionListingResponseDto> Listings { get; set; } = new();

    }

    public class PositionListingResponseDto
    {
        public string? UserName { get; set; }
        public string? CommodityName { get; set; }
        public decimal Quantity { get; set; }
        public string? UnitOfMeasure { get; set; }
    }
}
