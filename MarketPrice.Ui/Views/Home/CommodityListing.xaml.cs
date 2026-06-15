using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class CommodityListing : ContentPage
{
	public CommodityListing(CommodityListingViewModel vm)
	{
		BindingContext = vm; 
		InitializeComponent();
	}
}