using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;
using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class Home : ContentPage
{
    private readonly AuthenticationApiService _authenticationApiService;
    private readonly SessionService _sessionService;
    private readonly HomeViewModel _vm;

    public Home(
        AuthenticationApiService authenticationApiService,
        SessionService sessionService,
        HomeViewModel homeViewModel)
    {
        _authenticationApiService = authenticationApiService;
        _sessionService = sessionService;
        _vm = homeViewModel;

        BindingContext = homeViewModel;
        InitializeComponent();

        homeViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        System.Diagnostics.Debug.WriteLine($"[HOME PAGE] OnAppearing | BindingContext: {BindingContext?.GetType().Name}");

        try
        {
            bool isSessionValid = await _sessionService.ValidateAndRefreshSessionAsync();
            if (!isSessionValid)
            {
                bool refreshed = await _sessionService.TryRefreshTokenAsync();
                if (!refreshed)
                {
                    await Shell.Current.GoToAsync("//Welcome");
                    return;
                }
            }

            try { await _authenticationApiService.PingAsync(); }
            catch (Exception pingEx)
            {
                System.Diagnostics.Debug.WriteLine($"[HOME PAGE] Ping failed (non-fatal): {pingEx.Message}");
            }

            await _vm.InitializeAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[HOME PAGE] Exception: {ex.Message}");
            await Shell.Current.GoToAsync("//Welcome");
        }
    }

    private async void OnViewModelPropertyChanged(object sender,
        System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(HomeViewModel.IsSearchActive))
        {
            if (_vm.IsSearchActive)
                await ShowSearchOverlay();
            else
                await HideSearchOverlay();
        }
    }

    private async Task ShowSearchOverlay()
    {
        SearchOverlay.IsVisible = true;
        SearchOverlay.TranslationX = 400;
        await SearchOverlay.TranslateTo(0, 0, 250, Easing.CubicOut);

        // Small delay ensures layout is complete before focusing
        await Task.Delay(100);
        SearchEntry.Focus();
    }

    private async Task HideSearchOverlay()
    {
        await SearchOverlay.TranslateTo(400, 0, 200, Easing.CubicIn);
        SearchOverlay.IsVisible = false;
        SearchEntry.Unfocus();
    }


}