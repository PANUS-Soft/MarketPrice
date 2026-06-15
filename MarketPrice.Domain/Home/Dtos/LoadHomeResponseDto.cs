namespace MarketPrice.Domain.Home.DTOs
{
    public class LoadHomeResponseDto
    {
        public Guid CommodityTypeId { get; set; }
        public string? CommodityTypeName { get; set; }
        public List<HomeCommodityDetailDto> Commodities { get; set; } = new();
    }

    public class HomeCommodityDetailDto
    {
        public Guid CommodityId { get; set; }
        public string? CommodityName { get; set; }
        public Guid CommodityImageId { get; set; }
        public string? ImageUrl { get; set; }
        public short? LotSize { get; set; }
        public string? UnitOfMeasure { get; set; }
        public bool IsBidImproved { get; set; }
        public bool IsOfferImproved { get; set; }
        public bool IsBidSoonToExpire { get; set; }
        public bool IsOfferSoonToExpire { get; set; }
        public List<HomeMarketDepthDto> BidDepth { get; set; } = new();
        public List<HomeMarketDepthDto> OfferDepth { get; set; } = new();
    }

    public class HomeMarketDepthDto
    {
        public decimal Price { get; set; }
        public List<string> Locations { get; set; } = new();
        public decimal TotalActivePosforPrice { get; set; }
    }
}