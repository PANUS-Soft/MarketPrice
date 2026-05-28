using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class PlacePosition : ContentPage
{
	public PlacePosition(PlacePositionViewModel placePositionViewModel)
	{
		InitializeComponent();
		BindingContext = placePositionViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is PlacePositionViewModel placePositionViewModel)
        {
            await placePositionViewModel.LoadReferenceDataAsync();

            if (placePositionViewModel.ActivityToEdit != null)
            {
                // If we're editing an existing activity, load its details
                await placePositionViewModel.LoadFromActivity(placePositionViewModel.ActivityToEdit);
            }
        }
    }
}