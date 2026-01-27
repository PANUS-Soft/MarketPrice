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
        public decimal BestBid { get; set; }
        public decimal BestOffer { get; set; }
        public bool IsBidImproved { get; set; }
        public bool IsOfferImproved { get; set; }
        public string? UnitOfMeasure { get; set; }
    }


}
