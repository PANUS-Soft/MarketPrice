using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class ChangePhoneNumberIntro : ContentPage
{
	public ChangePhoneNumberIntro(ChangePhoneNumberIntroViewModel changePhoneNumberIntroViewModel )
	{
		InitializeComponent();
		BindingContext = changePhoneNumberIntroViewModel;
	}
}