using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class PlaceOffer : ContentPage
{
	public PlaceOffer(PositionViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
}