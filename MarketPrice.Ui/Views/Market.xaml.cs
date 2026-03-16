using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class Market : ContentPage
{
	public Market(MarketViewModel marketViewModel)
	{
		InitializeComponent();
        BindingContext = marketViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MarketViewModel marketViewModel) await marketViewModel.InitializeAsync();
    }
}