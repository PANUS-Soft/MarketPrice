using MarketPrice.Ui.Services.Api;

namespace MarketPrice.Ui.Views;

public partial class Home : ContentPage
{
    private readonly AuthenticationApiService _api;
	public Home(AuthenticationApiService api)
    {
        _api = api;
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

        try
        {
            await _api.PingAsync();
        }
        catch
        {
            await Shell.Current.GoToAsync("//Welcome");
        }
	}
}