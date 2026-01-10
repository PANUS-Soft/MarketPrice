using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class Register : ContentPage
{
    public Register(RegisterViewModel registerViewModel)
    {
        InitializeComponent();
        BindingContext = registerViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is RegisterViewModel viewModel)
        {
            //viewModel.ValidateCurrentStepRequested -= ValidateCurrentFormAsync; // Remove old one
            viewModel.ValidateCurrentStepRequested += ValidateCurrentFormAsync; // Add fresh one
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

        bool isValid = false;

        if (viewModel.IsPersonalStep)
            isValid = PersonalInfoForm.Validate();
        else if (viewModel.IsContactStep)
            isValid = ContactInfoForm.Validate();
        else if (viewModel.IsSecurityStep)
            isValid = SecurityDetailForm.Validate();

        return Task.FromResult(isValid);
    }

}