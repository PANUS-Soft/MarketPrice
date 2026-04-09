using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;
using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class Home : ContentPage
{
    private readonly AuthenticationApiService _authenticationApiService;
    private readonly SessionService _sessionService;

	public Home(AuthenticationApiService authenticationApiService, SessionService sessionService, HomeViewModel homeViewModel)
    {
        InitializeComponent();
        _authenticationApiService = authenticationApiService;
        _sessionService = sessionService;
        BindingContext = homeViewModel;
    }

	protected override async void OnAppearing()
	{
		base.OnAppearing();

        try
        {
            bool isSessionValid = await _sessionService.ValidateAndRefreshSessionAsync();
            if (!isSessionValid) await _sessionService.TryRefreshTokenAsync();

            //await _authenticationApiService.PingAsync();

            if (BindingContext is HomeViewModel homeViewModel)
            {
                await homeViewModel.InitializeAsync();

            }
        }
        catch
        {
            await Shell.Current.GoToAsync("//Welcome");
        }
	}
}