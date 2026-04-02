using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class ChangePassword : ContentPage
{
	public ChangePassword(ChangePasswordViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}