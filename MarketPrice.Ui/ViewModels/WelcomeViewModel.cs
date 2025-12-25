using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MarketPrice.Ui.ViewModels
{
    public class WelcomeViewModel : BindableObject

    {
        public ICommand NavigateToRegisterCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        public WelcomeViewModel()
        {
            NavigateToRegisterCommand = new Command(NavigateToRegister);
            NavigateToLoginCommand = new Command(NavigateToLogin);
        }

        private void NavigateToLogin()
        {
            Shell.Current.GoToAsync("//Login");
        }

        private void NavigateToRegister()
        {
            Shell.Current.GoToAsync("//Register");
        }
    }
}
