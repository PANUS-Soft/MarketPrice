using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class ChangeEmailAddressInput : ContentPage
{
	public ChangeEmailAddressInput(ChangeEmailAddressInputViewModel changeEmailAddressInputViewModel)
	{
        InitializeComponent();
        BindingContext = changeEmailAddressInputViewModel;
    }
}