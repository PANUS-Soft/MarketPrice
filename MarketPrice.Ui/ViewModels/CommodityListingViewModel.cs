using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevExpress.XtraEditors.Filtering;
using MarketPrice.Ui.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Ui.ViewModels
{
    [QueryProperty(nameof(GroupName), "GroupName")]
    [QueryProperty(nameof(CommodityList), "Commodities")]
    public partial class CommodityListingViewModel : ObservableObject
    {
            [ObservableProperty]
            private string _groupName = string.Empty;

            [ObservableProperty]
            private List<CommodityDisplayModel> _commodityList = new();

            public ObservableCollection<CommodityDisplayModel> Commodities { get; } = new();

            public string CommodityCount => Commodities.Count == 1
                ? "1 item in Category"
                : $"{Commodities.Count} items in category";

            partial void OnCommodityListChanged(List<CommodityDisplayModel> value)
            {
                Commodities.Clear();
                if (value == null) return;

                foreach (var item in value.OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase))
                    Commodities.Add(item);
                OnPropertyChanged(nameof(CommodityCount));
            }

            [RelayCommand]
            private async Task GoBack()
            {
                await Shell.Current.GoToAsync("..");
            }
    }
}
