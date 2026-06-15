namespace MarketPrice.Domain.Market.DTOs
{
    public class MarketInsightResponseDto
    {
        public Guid CommodityTypeId { get; set; }
        public Guid CommodityId { get; set; }
        public string CommodityName { get; set; }
        public decimal BestBid { get; set; }
        public decimal BestOffer { get; set; }
        public decimal MaxBid24H { get; set; }
        public decimal MaxOffer24H { get; set; }
        public decimal MinBid24H { get; set; }
        public decimal MinOffer24H { get; set; }
        public decimal BidPercentage { get; set; }
        public decimal OfferPercentage { get; set; }
        public decimal TotalMarketValue { get; set; }
        public List<MarketDepthItemDto> Bids {get; set;}
        public List<MarketDepthItemDto> Offers { get; set;}
    }

    public class MarketDepthItemDto
    {
        public int PositionCount { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
    }
}
