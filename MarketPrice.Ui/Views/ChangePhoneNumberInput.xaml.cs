using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class ChangePhoneNumberInput : ContentPage
{
	public ChangePhoneNumberInput(ChangePhoneNumberInputViewModel changePhoneNumberInputViewModel)
	{
		InitializeComponent();
        BindingContext = changePhoneNumberInputViewModel;
    }
}