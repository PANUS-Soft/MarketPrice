namespace MarketPrice.Ui.Models
{
    public class PendingNavigation
    {
        public string Route { get; set; } = string.Empty;
        public MarketItemFilter? MarketItemFilter { get; set; }
    }
}
