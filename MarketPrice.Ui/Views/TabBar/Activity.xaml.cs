using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class Activity : ContentPage
{
	public Activity(ActivityViewModel activityViewModel)
	{
		InitializeComponent();
		BindingContext = activityViewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ActivityViewModel activityViewModel)
        {
            await activityViewModel.InitializeAsync();
        }
    }
}
