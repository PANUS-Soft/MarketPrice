using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class Account : ContentPage
{
	public Account(AccountViewModel accountViewModel)
	{
		InitializeComponent();
		BindingContext = accountViewModel;
	}
}