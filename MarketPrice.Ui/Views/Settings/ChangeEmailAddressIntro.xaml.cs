using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class ChangeEmailAddressIntro : ContentPage
{
	public ChangeEmailAddressIntro(ChangeEmailAddressIntroViewModel changeEmailAddressIntroViewModel)
	{
		InitializeComponent();
        BindingContext = changeEmailAddressIntroViewModel;
    }
}