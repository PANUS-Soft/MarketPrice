using MarketPrice.Ui.ViewModels;
namespace MarketPrice.Ui.Views;

public partial class PlaceBid : ContentPage
{
	public PlaceBid()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is PlaceBidViewModel vm)
        {
            await vm.LoadInitialDataAsync();
        }
    }
}