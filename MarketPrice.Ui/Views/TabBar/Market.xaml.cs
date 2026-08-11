using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class Market : ContentPage
{
    private bool _initialized;
    public Market(MarketViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MarketViewModel marketViewModel)
        {
            if (!_initialized)
            {
                _initialized = true;
                await marketViewModel.InitializeAsync();
            }
            else
            {
                await marketViewModel.LoadMarketDataAsync();
            }
        }
    }
}
