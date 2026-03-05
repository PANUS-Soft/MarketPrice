using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class MarketInsight : ContentPage
{
	public MarketInsight(MarketInsightViewModel marketInsightViewModel)
	{
		InitializeComponent();
		BindingContext = marketInsightViewModel;
	}
}