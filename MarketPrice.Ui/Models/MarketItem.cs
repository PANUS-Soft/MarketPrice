namespace MarketPrice.Ui.Models
{
    public class MarketItem
    {
        public MarketItemFilter? Filter { get; set; }
        public ImageSource? ImageSource { get; set; }
        public short? LotSize { get; set; }
        public string? UnitOfMeasure { get; set; }
        public string? LotSizeDisplay { get; set; }

        public string? BestBid { get; set; }
        public decimal BestBidRaw { get; set; }
        public decimal BestBidQuantity { get; set; }
        public string? BestBidLocation { get; set; }

        public decimal NextBid1 { get; set; }
        public decimal NextBid2 { get; set; }

        public string? BestOffer { get; set; }
        public decimal BestOfferRaw { get; set; }
        public decimal BestOfferQuantity { get; set; }
        public string? BestOfferLocation { get; set; }

        public decimal NextOffer1 { get; set; }
        public decimal NextOffer2 { get; set; }

        public bool IsBidUp { get; set; }
        public bool IsBidDown { get; set; }
        public bool IsBidNull { get; set; }
        public bool IsOfferUp { get; set; }
        public bool IsOfferDown { get; set; }
        public bool IsOfferNull { get; set; }

        public bool IsBidSoonToExpire { get; set; }
        public bool IsOfferSoonToExpire { get; set; }
    }

    public class MarketItemFilter
    {
        public Guid CommodityTypeId { get; set; }
        public Guid CommodityId { get; set; }
        public string? Name { get; set; }
    }
}