using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;
using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class Home : ContentPage
{
    private readonly AuthenticationApiService _authenticationApiService;
    private readonly SessionService _sessionService;
    private readonly HomeViewModel _homeViewModel;

	public Home(AuthenticationApiService authenticationApiService, SessionService sessionService, HomeViewModel homeViewModel)
    {
        
        _authenticationApiService = authenticationApiService;
        _sessionService = sessionService;
        _homeViewModel = homeViewModel;

        BindingContext = homeViewModel;
        InitializeComponent();
    }

	protected override async void OnAppearing()
	{
		base.OnAppearing();
        System.Diagnostics.Debug.WriteLine($"[HOME PAGE] OnAppearing | BindingContext: {BindingContext?.GetType().Name}");

        try
        {
            bool isSessionValid = await _sessionService.ValidateAndRefreshSessionAsync();
            System.Diagnostics.Debug.WriteLine($"[HOME PAGE] Session valid: {isSessionValid}");

            if (!isSessionValid)
            {
                bool refreshed = await _sessionService.TryRefreshTokenAsync();
                System.Diagnostics.Debug.WriteLine($"[HOME PAGE] Token refreshed: {refreshed}");

                if (!refreshed)
                {
                    System.Diagnostics.Debug.WriteLine("[HOME PAGE] Session invalid, redirecting to Welcome");
                    await Shell.Current.GoToAsync("//Welcome");
                    return;
                }
            }

            try
            {
                await _authenticationApiService.PingAsync();
                System.Diagnostics.Debug.WriteLine("[HOME PAGE] Ping succeeded");
            }
            catch(Exception pingEx)
            {
                System.Diagnostics.Debug.WriteLine($"[HOME PAGE] Ping failed: {pingEx.Message}");
            }

            System.Diagnostics.Debug.WriteLine("[HOME PAGE] Loading home data...");
            await _homeViewModel.InitializeAsync();
            System.Diagnostics.Debug.WriteLine($"[HOME PAGE] ✅ Done | Commodities: {_homeViewModel.Commodities.Count}");


        }
        catch(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[HOME PAGE] EXCEPTION: {ex.GetType().Name}: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[HOME PAGE] Redirecting to Welcome");
            await Shell.Current.GoToAsync("//Welcome");
        }
	}
}