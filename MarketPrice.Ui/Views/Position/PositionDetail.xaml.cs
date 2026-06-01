using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class PositionDetail : ContentPage
{
	public PositionDetail(PositionDetailViewModel positionDetailViewModel)
	{
		InitializeComponent();
        BindingContext = positionDetailViewModel;
    }
}