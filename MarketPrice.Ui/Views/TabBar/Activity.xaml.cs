using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class Activity : ContentPage
{
	public Activity(ActivityViewModel activityViewModel)
	{
		InitializeComponent();
		BindingContext = activityViewModel;
	}
}