using MarketPrice.Domain.Position.Commands;

namespace MarketPrice.Ui.Models
{
    public class PendingNavigation
    {
        public string Route { get; set; } = string.Empty;
        public MarketItemFilter? MarketItemFilter { get; set; }
        public PositionListingCommand? PositionListingCommand { get; set; }
        public string? PassedImageLocation { get; set; }
        public string? PassedCommodityName { get; set; }
        public string? PassedLotSize { get; set; }
        public string? PassedBid { get; set; }
        public string? PassedOffer { get; set; }
    }
}
