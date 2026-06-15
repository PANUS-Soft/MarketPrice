namespace MarketPrice.Ui.Models;

public class HomeDisplayInformation
{
    public Guid CommodityTypeId { get; set; }
    public string? Name { get; set; }
    public ImageSource? ImageSource { get; set; }
    public Color? BackgroundColor { get; set; }
    public string? LotSize { get; set; }
    public bool IsBidTrendUp { get; set; }
    public bool IsBidTrendDown { get; set; }
    public bool IsOfferTrendUp { get; set; }
    public bool IsOfferTrendDown { get; set; }
    public bool IsBidSoonToExpire { get; set; }
    public bool IsOfferSoonToExpire { get; set; }
    public List<HomeMarketDepth>? BidDepth { get; set; }
    public List<HomeMarketDepth>? OfferDepth { get; set; }
}

public class HomeMarketDepth
{
    public decimal Price { get; set; }
    public List<string>? Locations { get; set; }
    public int TotalActivesPositions { get; set; }
}