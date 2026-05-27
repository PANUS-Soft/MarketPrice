using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Ui.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MarketPrice.Ui.ViewModels
{
    public partial class ActivityViewModel : ObservableObject
    {
        private List<Activity> _allActivities = new();

        // 1. Loading States
        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string searchText;

        [ObservableProperty]
        private string selectedPositionType = "All";

        [ObservableProperty]
        private string selectedCommodityType = "All";

        public ObservableCollection<string> PositionTypes { get; } = new() { "All", "Bids", "Offers" };
        public ObservableCollection<string> CommodityTypes { get; } = new() { "All", "Beans", "Egusi", "P. Oil", "Onion", "Ginger" };
        public ObservableCollection<ActivityGroup> GroupedActivities { get; } = new();

        // 2. A simple dummy list to generate 5 skeleton cards
        public ObservableCollection<int> SkeletonItems { get; } = new() { 1, 2, 3, 4, 5 };

        public ActivityViewModel()
        {
            // Fire and forget the loading task
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            // Turn on the loading skeleton
            IsLoading = true;

            // Simulate a 2-second network/database delay
            await Task.Delay(2000);

            _allActivities = new List<Activity>
            {
                new() { CommodityName = "White Beans", Quantity = "10 bags", Price = "250,000 FCFA", State = "Pending", ImageUrl = "corn_placeholder.png", Date = DateTime.Now, PositionType = "Bid" },
                new() { CommodityName = "Red Beans", Quantity = "5 bags", Price = "125,000 FCFA", State = "Completed", ImageUrl = "corn_placeholder.png", Date = DateTime.Now, PositionType = "Offer" },
                new() { CommodityName = "Egusi", Quantity = "20 bags", Price = "500,000 FCFA", State = "Completed", ImageUrl = "corn_placeholder.png", Date = DateTime.Now.AddDays(-1), PositionType = "Bid" },
                new() { CommodityName = "White Beans", Quantity = "2 bags", Price = "50,000 FCFA", State = "Cancelled", ImageUrl = "corn_placeholder.png", Date = DateTime.Now.AddDays(-1), PositionType = "Offer" },
                new() { CommodityName = "P. Oil", Quantity = "50 Liters", Price = "40,000 FCFA", State = "Completed", ImageUrl = "corn_placeholder.png", Date = DateTime.Now.AddDays(-5), PositionType = "Bid" },
                new() { CommodityName = "Egusi", Quantity = "10 bags", Price = "250,000 FCFA", State = "Pending", ImageUrl = "corn_placeholder.png", Date = DateTime.Now.AddDays(-6), PositionType = "Offer" }
            };

            ApplyFilters();

            // Turn off the skeleton and show the real data
            IsLoading = false;
        }

        partial void OnSearchTextChanged(string value) => ApplyFilters();
        partial void OnSelectedPositionTypeChanged(string value) => ApplyFilters();

        private string _lastCommodityType = "All";

        partial void OnSelectedCommodityTypeChanged(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                MainThread.BeginInvokeOnMainThread(() => SelectedCommodityType = _lastCommodityType);
                return;
            }
            _lastCommodityType = value;
            ApplyFilters();
        }

        [RelayCommand]
        private void ApplyFilters()
        {
            if (_allActivities == null || !_allActivities.Any()) return;

            var filteredData = _allActivities.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
                filteredData = filteredData.Where(x => x.CommodityName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(SelectedPositionType) && SelectedPositionType != "All")
            {
                string targetType = SelectedPositionType == "Bids" ? "Bid" : "Offer";
                filteredData = filteredData.Where(x => x.PositionType.Equals(targetType, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(SelectedCommodityType) && SelectedCommodityType != "All")
                filteredData = filteredData.Where(x => x.CommodityName.Contains(SelectedCommodityType, StringComparison.OrdinalIgnoreCase));

            var today = filteredData.Where(x => x.Date.Date == DateTime.Now.Date).ToList();
            var yesterday = filteredData.Where(x => x.Date.Date == DateTime.Now.AddDays(-1).Date).ToList();
            var lastWeek = filteredData.Where(x => x.Date.Date < DateTime.Now.AddDays(-1).Date).ToList();

            GroupedActivities.Clear();
            if (today.Any()) GroupedActivities.Add(new ActivityGroup("Today", today));
            if (yesterday.Any()) GroupedActivities.Add(new ActivityGroup("Yesterday", yesterday));
            if (lastWeek.Any()) GroupedActivities.Add(new ActivityGroup("Last week", lastWeek));
        }

        [RelayCommand]
        private async Task GoToDetailsAsync(MarketPrice.Ui.Models.Activity selectedItem)
        {
            if (selectedItem == null) return;
            // var navigationParameter = new Dictionary<string, object> { { "ActivityDetail", selectedItem } };
            // await Shell.Current.GoToAsync(nameof(Views.PositionDetail), navigationParameter);
        }
    }
}