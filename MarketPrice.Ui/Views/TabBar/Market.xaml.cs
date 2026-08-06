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

        if (_initialized)
            return;

        _initialized = true;

        if (BindingContext is MarketViewModel marketViewModel)
        {
            //while (marketViewModel.IsLoading)
            //{
            //    await this.FadeTo(0.5, 800, Easing.CubicIn);
            //    await this.FadeTo(1.0, 800, Easing.CubicOut);
            //}
            await marketViewModel.InitializeAsync();
        }
    }
}