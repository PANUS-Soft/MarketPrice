using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class Market : ContentPage
{
	public Market(MarketViewModel marketViewModel)
	{
		InitializeComponent();
        BindingContext = marketViewModel;
    }
}