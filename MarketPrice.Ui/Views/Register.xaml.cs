using MarketPrice.Ui.Models;
using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class Register : ContentPage
{
	public Register()
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

        if (viewModel.IsPersonalStep)
            PersonalInfoForm.Commit();
        else if (viewModel.IsContactStep)
            ContactInfoForm.Commit();
        else if (viewModel.IsSecurityStep)
            SecurityDetailForm.Commit();

        bool isValid = 
            viewModel.IsPersonalStep && PersonalInfoForm.Validate() ||
            viewModel.IsContactStep && ContactInfoForm.Validate() ||
            viewModel.IsSecurityStep && SecurityDetailForm.Validate();

        return Task.FromResult(isValid);
    }

}