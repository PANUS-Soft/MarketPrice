using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketPrice.Ui.Services.Session;

namespace MarketPrice.Ui.ViewModels
{
    public partial class ActivityViewModel : ObservableObject
    {
        private readonly SessionService _sessionService;
        public ObservableCollection<Commodity>? Position { get; set; }
        public ObservableCollection<string>?  CommodityTypes { get; set; }

        public ActivityViewModel(SessionService sessionService)
        {
            _sessionService = sessionService;

            CommodityTypes = new ObservableCollection<string> { "Corn", "Bean", "Ginger", "Onion", "Palm Oil", "Egusi" };

            Position = new ObservableCollection<Commodity>()
            {
                new Commodity{Name = "Fresh Corn", Price = "2,500", Quantity = "10 units", Grade = "Grade A"},
                new Commodity{Name = "Dry Corn", Price = "3,200", Quantity = "5 units", Grade = "Grade B"},
            };
        }

        [RelayCommand]
        private async Task NavigateToLoginAsync()
        {
            await Shell.Current.GoToAsync("//Login");
        }

        [RelayCommand]
        private async Task NavigateToWRegisterAsync()
        {
            await Shell.Current.GoToAsync("//Register");
        }

        public bool IsUserLoggedIn => _sessionService.IsLoggedIn;

        [RelayCommand]
        async Task Edit(Commodity item)
        {
            if (item  == null) return;

            await Shell.Current.GoToAsync($"{nameof(EditPosition)}?CommodityName{item.Name}");
        }
    }
}
