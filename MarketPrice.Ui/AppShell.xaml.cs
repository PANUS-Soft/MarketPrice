using MarketPrice.Ui.Views;

namespace MarketPrice.Ui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MarketInsight), typeof(MarketInsight));
            Routing.RegisterRoute(nameof(PlacePosition), typeof(PlacePosition));
            Routing.RegisterRoute(nameof(PositionListing), typeof(PositionListing));
            Routing.RegisterRoute(nameof(PositionDetail), typeof(PositionDetail));
            Routing.RegisterRoute(nameof(EditPosition), typeof(EditPosition));
            Routing.RegisterRoute(nameof(EditProfile), typeof(EditProfile));
            Routing.RegisterRoute(nameof(ChangePassword), typeof(ChangePassword));
            Routing.RegisterRoute(nameof(CommodityListing), typeof(CommodityListing));
            Routing.RegisterRoute(nameof(Settings), typeof(Settings));
        }
    }
}
