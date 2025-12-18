using MarketPrice.Ui.Models;
using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class Register2 : ContentPage
{
	public Register2()
	{
		InitializeComponent();
        BindingContext = new RegisterViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is RegisterViewModel viewModel)
        {
            viewModel.ValidateCurrentStepRequested += ValidateCurrentFormAsync;
        }
    }

    private Task<bool> ValidateCurrentFormAsync()
    {
        if (BindingContext is not RegisterViewModel viewModel)
            return Task.FromResult(false);

        bool isValid = 
            viewModel.IsPersonalStep && PersonalInfoForm.Validate() ||
            viewModel.IsContactStep && ContactInfoForm.Validate() ||
            viewModel.IsSecurityStep && SecurityDetailForm.Validate();

        return Task.FromResult(isValid);
    }


    private void OnStepOneTapped(object? sender, TappedEventArgs e)
    {
    }

    private void OnStepTwoTapped(object? sender, TappedEventArgs e)
    {
    }
}