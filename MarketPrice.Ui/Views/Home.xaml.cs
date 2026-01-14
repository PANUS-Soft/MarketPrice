using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;

namespace MarketPrice.Ui.Views;

public partial class Home : ContentPage
{
    private readonly AuthenticationApiService _authenticationApiService;
    private readonly SessionService _sessionService;

	public Home(AuthenticationApiService authenticationApiService, SessionService sessionService)
    {
        _authenticationApiService = authenticationApiService;
        _sessionService = sessionService;
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

        try
        {
            await _sessionService.ValidateAndRefreshSessionAsync();
            await _authenticationApiService.PingAsync();
        }
        catch
        {
            await Shell.Current.GoToAsync("//Welcome");
        }
	}
}