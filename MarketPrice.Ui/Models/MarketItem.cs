namespace MarketPrice.Ui.Models
{
    public class MarketItem
    {
        public string? Name { get; set; }
        public ImageSource ImageSource { get; set; }
        public short? LotSize { get; set; }
        public decimal BestBid { get; set; }
        public decimal BestOffer { get; set; }
        public bool IsBidUp { get; set; }
        public bool IsBidDown { get; set; }
        public bool IsOfferUp { get; set; }
        public bool IsOfferDown { get; set; }
        public string? UnitOfMeasure { get; set; }
        public string? LotSizeDisplay { get; set; }
    }
}
