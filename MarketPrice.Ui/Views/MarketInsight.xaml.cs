using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui;

public partial class MarketInsight : ContentPage
{
	public MarketInsight(MarketInsightViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}