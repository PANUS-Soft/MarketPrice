using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class Settings : ContentPage
{
    private readonly SettingsViewModel _settingsViewModel;

	public Settings(SettingsViewModel settingsViewModel)
	{
		InitializeComponent();
		BindingContext = settingsViewModel;
        _settingsViewModel = settingsViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

		try
		{
			_settingsViewModel.LoadUserProfileAsync();
		}
		catch
        {
            await Shell.Current.DisplayAlert("Error ??", "There was an error loading data.", "OK");
        }
	}
}