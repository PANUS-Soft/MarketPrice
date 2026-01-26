namespace MarketPrice.Ui.Models;

public class CommodityDisplayItem
{
    public string Name { get; set; }
    public string ImageSource { get; set; }
    public Color BackgroundColor { get; set; }
    public string LotSize { get; set; }
    public string BestBidPrice { get; set; }
    public bool IsBidTrendUp { get; set; }
    public string BestOfferPrice { get; set; }
    public bool IsOfferTrendUp { get; set; }
}