using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class Profile : ContentPage
{
	public Profile(ProfileViewModel profileViewModel)
	{
		InitializeComponent();
        BindingContext = profileViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ProfileViewModel profileViewModel) await profileViewModel.RefreshAsync();
    }
}
