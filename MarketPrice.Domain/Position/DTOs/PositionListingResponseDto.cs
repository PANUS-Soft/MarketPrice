using MarketPrice.Domain.Authentication.DTOs;

namespace MarketPrice.Domain.Position.DTOs
{
    public class PositionListingResponseDto:BaseResponseDto
    {
        public string? CommodityTypeName { get; set; } 
        public string? PositionTypeName { get; set; }
        public decimal UnitPrice { get; set; }
        public string LotSize { get; set; }
        public string? ShelfLife { get; set; }
        public List<string> CommodityNames { get; set; } = new();
        public List<PositionListing> Listings { get; set; } = new();
        public List<PricePointDto> BidPrices { get; set; } = new();
        public List<PricePointDto> OfferPrices { get; set; } = new();

    }

    public class PricePointDto
    {
        public decimal Price { get; set; }
        public int Count { get; set; }
    }

    public class PositionListing
    {
        public Guid PositionId { get; set; }
        public string? UserName { get; set; }
        public string? CommodityName { get; set; }
        public decimal Quantity { get; set; }
        public string? UnitOfMeasure { get; set; }
        public int? ShelfLifeInDays { get; set; }
        public string? LocationName { get; set; }
        public string? OriginTown { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsExpiringSoon { get; set; }
        public string ExpiryText { get; set; } = string.Empty;
        public bool IsCriticalExpiry { get; set; }
        public bool IsDeliverable { get; set; }
        public decimal? DeliveryFee { get; set; }

    }
}
