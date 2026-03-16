using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Ui.ViewModels
{
    public partial class EditPositionViewModel : ObservableObject
    {
        [ObservableProperty] bool isDetailExpanded = true;
        [ObservableProperty] bool isTimingExpanded = false;
        [ObservableProperty] bool isLogisticExpanded = false;

        [RelayCommand]
        void ToggleDetails()
        {
            IsDetailExpanded = !IsDetailExpanded;
            if (IsDetailExpanded)
            {
                IsTimingExpanded = false; 
                IsLogisticExpanded = false;
            }
        }

        [RelayCommand]
        void ToggleTiming()
        {
            IsTimingExpanded = !IsTimingExpanded;
            if (!IsTimingExpanded)
            {
                IsDetailExpanded = false;
                IsLogisticExpanded= false;
            }
        }

        [RelayCommand]
        void ToggleLogistic()
        {
            IsLogisticExpanded = !IsLogisticExpanded;
            if (!IsLogisticExpanded)
            {
                IsDetailExpanded = false;
                IsTimingExpanded= false;
            }
        }

        [RelayCommand]
        async Task GoBack() => await Shell.Current.GoToAsync("..");
    }
}
