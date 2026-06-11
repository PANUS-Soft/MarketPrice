using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class Privacy : ContentPage
{
	public Privacy(PrivacyViewModel privacyViewModel)
	{
		InitializeComponent();
		BindingContext = privacyViewModel;

	}


}