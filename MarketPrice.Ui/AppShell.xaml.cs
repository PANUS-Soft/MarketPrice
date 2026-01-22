using MarketPrice.Ui.Views;

namespace MarketPrice.Ui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(PlacePosition), typeof(PlacePosition));
        }
    }
}
