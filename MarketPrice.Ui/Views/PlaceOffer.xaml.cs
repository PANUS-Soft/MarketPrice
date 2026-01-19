using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class PlaceOffer : ContentPage
{
	public PlaceOffer(PositionViewModel positionViewModel)
	{
		InitializeComponent();
		BindingContext = positionViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is PositionViewModel positionViewModel)
        {
            positionViewModel.ValidateCurrentStepRequested += ValidateCurrentFormAsync();
        }
    }

    private Func<Task<bool>>? ValidateCurrentFormAsync()
    {
        //if (BindingContext is not PositionViewModel positionViewModel)
        //    return Task.FromResult(false);
        return null;
    }
}