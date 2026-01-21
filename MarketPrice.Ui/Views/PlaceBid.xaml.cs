using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class PlaceBid : ContentPage
{
    
    public PlaceBid()
    {
        InitializeComponent();
    }

    public PlaceBid(PlacePositionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    //protected override async void OnAppearing()
    //{
    //    base.OnAppearing();

        
    //    if (BindingContext is PositionViewModel vm)
    //    {
    //        await vm.LoadInitialDataAsync();
    //    }
    //}
}