using MarketPrice.Ui.ViewModels;
namespace MarketPrice.Ui.Views;

public partial class Verification : ContentPage
{
	public Verification()
	{
		InitializeComponent();
        BindingContext = new VerificationViewModel();
    }
}