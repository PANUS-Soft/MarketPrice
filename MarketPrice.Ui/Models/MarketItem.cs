namespace MarketPrice.Ui.Models
{
    public class MarketItem
    {
        public MarketItemFilter Filter { get; set; }
        public ImageSource? ImageSource { get; set; }
        public short? LotSize { get; set; }
        public string? BestBid { get; set; }
        public decimal HighBid { get; set; }
        public decimal LowBid { get; set; }
        public string? BestOffer { get; set; }
        public decimal HighOffer { get; set; }
        public decimal LowOffer { get; set; }
        public bool IsBidUp { get; set; }
        public bool IsBidDown { get; set; }
        public bool IsOfferUp { get; set; }
        public bool IsOfferDown { get; set; }
        public string? UnitOfMeasure { get; set; }
        public string? LotSizeDisplay { get; set; }
        public bool IsBidNull { get; set; }
        public bool IsOfferNull { get; set; }
    }

    public class MarketItemFilter
    {
        public Guid CommodityTypeId { get; set; }
        public Guid CommodityId { get; set; }
        public string? Name { get; set; }
    }
}