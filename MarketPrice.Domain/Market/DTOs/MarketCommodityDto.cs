namespace MarketPrice.Domain.Market.DTOs
{
    public class MarketCommodityDto
    {
        public Guid CommodityId { get; set; }
        public string CommodityName { get; set; } = string.Empty;
        public string LotSizeDisplay { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal PriceDifference { get; set; }

        // Tells the UI whether to make the badge Green or Red
        public bool IsPositiveTrend => PriceDifference >= 0;

        // Formats the text inside the badge (e.g., "+550" or "-550")
        public string FormattedDifference => PriceDifference > 0
            ? $"+{PriceDifference:N0}"
            : $"{PriceDifference:N0}";
    }
}