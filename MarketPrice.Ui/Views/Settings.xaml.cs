using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class Settings : ContentPage
{
	public Settings(SettingsViewModel settingsViewModel)
	{
		InitializeComponent();
		BindingContext = settingsViewModel;
	}
}