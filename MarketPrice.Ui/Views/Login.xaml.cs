using MarketPrice.Ui.ViewModels;

namespace MarketPrice.Ui.Views;

public partial class Login : ContentPage
{
    public Login(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}