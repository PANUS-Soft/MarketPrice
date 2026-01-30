using MarketPrice.Domain.Authentication.DTOs;

namespace MarketPrice.Domain.Position.DTOs
{
    public class PositionListingResponseDto:BaseResponseDto
    {
        public string? CommodityTypeName { get; set; } 
        public string? PositionTypeName { get; set; }
        public decimal UnitPrice { get; set; }
        public string LotSize { get; set; }
        public List<string> CommodityNames { get; set; } = new();
        public List<PositionListing> Listings { get; set; } = new();

    }

    public class PositionListing
    {
        public string? UserName { get; set; }
        public string? CommodityName { get; set; }
        public decimal Quantity { get; set; }
        public string? UnitOfMeasure { get; set; }
    }
}
