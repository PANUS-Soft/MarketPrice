using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class EditProfile : ContentPage
{
	public EditProfile(EditProfileViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}