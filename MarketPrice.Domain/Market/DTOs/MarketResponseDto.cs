namespace MarketPrice.Domain.Market.DTOs
{
    public class MarketResponseDto
    {
        public Guid CommodityId { get; set; }
        public Guid CommodityTypeId { get; set; }
        public string? CommodityName { get; set; }
        public Guid CommodityImageId { get; set; }
        public short? LotSize { get; set; }
        public string? ImageUrl { get; set; }
        public string? UnitOfMeasure { get; set; }
        public bool IsBidImproved { get; set; }
        public bool IsOfferImproved { get; set; }
        public decimal BestBid { get; set; }
        public decimal BestOffer { get; set; }
        public bool IsBestBidSoonToExpire { get; set; }
        public bool IsBestOfferSoonToExpire { get; set; }
        public List<MarketDepthDto> BidDepth { get; set; } = new();
        public List<MarketDepthDto> OfferDepth { get; set; } = new();
    }

    public class MarketDepthDto
    {
        public decimal Price { get; set; }
        public List<string> Locations { get; set; } = new();
        public decimal TotalActivePosforPrice { get; set; }
    }

}
