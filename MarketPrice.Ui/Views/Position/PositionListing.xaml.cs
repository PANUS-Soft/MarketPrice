using MarketPrice.Domain.Position.Commands;
using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

[QueryProperty(nameof(Args), "Args")]
public partial class PositionListing : ContentPage
{
    public PositionListingCommand Args
    {
        set
        {
            if (BindingContext is PositionListingViewModel positionListingViewModel)
            {
                _ = positionListingViewModel.InitializeAsync(value);
            }
        }
    }

	public PositionListing(PositionListingViewModel positionListingViewModel)
	{
		InitializeComponent();
        BindingContext = positionListingViewModel;
    }
}