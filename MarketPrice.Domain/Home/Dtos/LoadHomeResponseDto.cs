namespace MarketPrice.Domain.Home.Dtos
{
    public  class LoadHomeResponseDto
    {
        public Guid CommodityTypeId { get; set; }
        public Guid CommodityTypeImageId { get; set; }
        public string? CommodityTypeName { get; set; }
        public string? ImageUrl { get; set; } 
        public decimal LotSize { get; set; }
        public string? UnitOfMeasure { get; set; }
        public decimal BestBidPrice { get; set; }
        public decimal BestOfferPrice { get; set; }
        public bool IsBidImproved { get; set; }
        public bool IsOfferImproved { get; set; }
    }
}
