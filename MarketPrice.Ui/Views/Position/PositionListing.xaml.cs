using MarketPrice.Domain.Position.Commands;
using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class PositionListing : ContentPage, IQueryAttributable
{
	public PositionListing(PositionListingViewModel positionListingViewModel)
	{
		InitializeComponent();
        BindingContext = positionListingViewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (BindingContext is PositionListingViewModel positionListingViewModel)
        {
            positionListingViewModel.ApplyQueryAttributes(query);
        }
    }
}
