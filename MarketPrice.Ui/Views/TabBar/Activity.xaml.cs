using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class Activity : ContentPage
{
	private readonly ActivityViewModel _activityViewModel;

	public Activity(ActivityViewModel activityViewModel)
	{
		InitializeComponent();
		_activityViewModel = activityViewModel;
		BindingContext = activityViewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _activityViewModel.RefreshAsync();
	}
}
