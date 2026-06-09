using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Domain.Reference.DTOs;
using MarketPrice.Ui.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DevExpress.Maui.Controls;
using MarketPrice.Domain.Activity.DTOs;
using MarketPrice.Ui.Common;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;
using MarketPrice.Ui.Views;
using Activity = MarketPrice.Ui.Models.Activity;

namespace MarketPrice.Ui.ViewModels
{
    public partial class ActivityViewModel : ObservableObject
    {
        private readonly ActivityApiService _activityApiService;
        private readonly SessionService _sessionService;
        private readonly ReferenceDataApiService _referenceDataApi;

        private List<ActivityResponseDto> _allActivities = new();

        public ObservableCollection<string> CommodityTypesList { get; } =
            new(); // The list of commodity types that will serve for filtering

        [ObservableProperty] private BottomSheetState _filterBottomSheetState = BottomSheetState.Hidden;

        [ObservableProperty] private string searchText;
        [ObservableProperty] private string selectedPositionType = "All";
        [ObservableProperty] private string selectedCommodityType = "ALL";
        [ObservableProperty] private Activity selectedItem;
        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private BottomSheetState _activityDetailsBottomSheetState = BottomSheetState.Hidden;
        [ObservableProperty] private Activity selectedActivityDetails;

        public ObservableCollection<string> PositionTypes { get; } = new() { "All", "Bids", "Offers" };

        public ObservableCollection<ActivityGroup> GroupedActivities { get; } = new();

        public ActivityViewModel(ActivityApiService activityApiService, SessionService sessionService, ReferenceDataApiService referenceDataApiService)
        {
            _referenceDataApi = referenceDataApiService;
            _sessionService = sessionService;
            _activityApiService = activityApiService;

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await LoadCommodityTypesAsync();
            await LoadUserActivityAsync();
        }

        private async Task LoadCommodityTypesAsync()
        {
            var response = await _referenceDataApi.GetCommodityTypesAsync();

            if (!response.IsSuccessStatusCode) return;

            var commodityTypes = await response.Content.ReadFromJsonAsync<List<CommodityTypeDto>>();

            if (commodityTypes == null) return;

            CommodityTypesList.Clear();
            CommodityTypesList.Add("ALL");

            foreach (var type in commodityTypes.OrderBy(t => t.Name, StringComparer.OrdinalIgnoreCase))
                CommodityTypesList.Add(type.Name!.ToUpper());

            SelectedCommodityType = "ALL";
        }

        private async Task LoadUserActivityAsync()
        {
            try
            {
                IsLoading = true;

                var userSession = await _sessionService.GetCurrentSessionAsync();

                if (userSession == null) return;

                var response = await _activityApiService.GetUserActivityAsync(userSession.UserId);

                if (!response.IsSuccessStatusCode) return;

                var dto = await response.Content.ReadFromJsonAsync<ActivityGroupDto>();

                if (dto == null) return;

                _allActivities = new List<ActivityResponseDto>();

                if (dto.Today != null) _allActivities.AddRange(dto.Today);
                if (dto.Yesterday != null) _allActivities.AddRange(dto.Yesterday);
                if (dto.ThisWeek != null) _allActivities.AddRange(dto.ThisWeek);
                if (dto.LastWeek != null) _allActivities.AddRange(dto.LastWeek);
                if (dto.ThisMonth != null) _allActivities.AddRange(dto.ThisMonth);
                if (dto.LastMonth != null) _allActivities.AddRange(dto.LastMonth);

                ApplyFilters();
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error ⚠️",
                    $"[ActivityViewModel] failed to load activity: {e.Message}",
                    "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private static Activity MapToUiModel(ActivityResponseDto dto) => new()
        {
            ActivityResponse = dto,
            PositionId = dto.PositionId,
            CommodityId = dto.CommodityId,
            CommodityTypeId = dto.CommodityTypeId,
            CommodityName = dto.CommodityName,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Date = dto.CreatedAt.DateTime,
            Description = dto.Description,
            OriginRegion = dto.OriginRegion,
            DestinationRegion = dto.DestinationRegion,
            Origin = dto.Origin,
            Destination = dto.Destination,
            Grade = dto.Grade,
            LeadTime = string.IsNullOrEmpty(dto.LeadTime) ? string.Empty : $"{dto.LeadTime} Days",
            DeliveryFee = dto.DeliveryFee.HasValue ? $"{dto.DeliveryFee:N0} FCFA" : string.Empty,
            ShelfLifeInDays = dto.ShelfLifeInDays,
            IsDeliverable = dto.CanDeliver,
            Quantity = $"{dto.Quantity:N0}",
            TotalQuantity = $"{(dto.Quantity * dto.LotSize):N0} {dto.UnitOfMeasure}",
            Price = $"{dto.UnitPrice:N0} FCFA",
            TotalPrice = $"{(dto.Quantity * dto.UnitPrice):N0} FCFA",
            State = dto.State,
            StateColor = dto.State == "Open" ? Color.FromArgb("#2ECC71") : dto.State == "Close" ? Color.FromArgb("#E74C3C") : Color.FromArgb("#F39C12"),
            PosType = dto.PositionType,
            PositionType = dto.PositionType == "Bid" ? PositionType.Bid : PositionType.Offer,
            LotSize = $"{dto.LotSize} {dto.UnitOfMeasure}",
            UnitOfMeasure = dto.UnitOfMeasure,
        };

        partial void OnSearchTextChanged(string value) => ApplyFilters();
        partial void OnSelectedItemChanged(Activity value)
        {
            if (value == null) return;

            FilterBottomSheetState = BottomSheetState.Hidden;

            SelectedActivityDetails = value;            
            ActivityDetailsBottomSheetState = BottomSheetState.HalfExpanded;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                SelectedItem = null;
            });
        }


        partial void OnSelectedPositionTypeChanged(string value) => ApplyFilters();

        private string _lastCommodityType = "ALL";

        partial void OnSelectedCommodityTypeChanged(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                MainThread.BeginInvokeOnMainThread(() => { SelectedCommodityType = _lastCommodityType; });
                return;
            }
            _lastCommodityType = value;
            ApplyFilters();
        }

        [RelayCommand]
        private void ShowFilters()
        {
            ActivityDetailsBottomSheetState = BottomSheetState.Hidden;
            FilterBottomSheetState = BottomSheetState.HalfExpanded;
        }

        [RelayCommand]
        private void ApplyFilterAndClose()
        {
            ApplyFilters();
            FilterBottomSheetState = BottomSheetState.Hidden;
        }

        [RelayCommand]
        private void ClearFilters()
        {
            SelectedPositionType = "All";
            FilterBottomSheetState = BottomSheetState.Hidden;
        }

        [RelayCommand]
        private void ApplyFilters()
        {
            if (!_allActivities.Any())
            {
                GroupedActivities.Clear();
                return;
            }

            IEnumerable<ActivityResponseDto> filteredActivities = _allActivities;

            if (!string.IsNullOrWhiteSpace(SearchText))
                filteredActivities = filteredActivities.Where(x =>
                    x.CommodityName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(SelectedPositionType) && SelectedPositionType != "All")
            {
                var target = SelectedPositionType == "Bids" ? "Bid" : "Offer";
                filteredActivities =
                    filteredActivities.Where(x => x.PositionType.Equals(target, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(SelectedCommodityType) && SelectedCommodityType != "ALL")
                filteredActivities = filteredActivities.Where(x =>
                    x.CommodityName.Contains(SelectedCommodityType, StringComparison.OrdinalIgnoreCase));

            var uiItems = filteredActivities.Select(MapToUiModel).ToList();

            var now = DateTime.Now.Date;
            var startOfThisWeek = now.AddDays(-(int)now.DayOfWeek == 0 ? 6 : (int)now.DayOfWeek - 1);
            var startOfLastWeek = startOfThisWeek.AddDays(-7);
            var startOfThisMonth = new DateTime(now.Year, now.Month, 1);
            var startOfLastMonth = startOfThisMonth.AddMonths(-1);

            var today = uiItems.Where(x => x.Date.Date == now).ToList();
            var yesterday = uiItems.Where(x => x.Date.Date == now.AddDays(-1)).ToList();
            var thisWeek = uiItems.Where(x => x.Date.Date >= startOfThisWeek && x.Date.Date < now.AddDays(-1)).ToList();
            var lastWeek = uiItems.Where(x => x.Date.Date >= startOfLastWeek && x.Date.Date < startOfThisWeek).ToList();
            var thisMonth = uiItems.Where(x => x.Date.Date >= startOfThisMonth && x.Date.Date < startOfLastWeek).ToList();
            var lastMonth = uiItems.Where(x => x.Date.Date >= startOfLastMonth && x.Date.Date < startOfThisMonth).ToList();
            var older = uiItems.Where(x => x.Date.Date < startOfLastMonth).ToList();


            MainThread.BeginInvokeOnMainThread(() =>
            {
                GroupedActivities.Clear();
                if (today.Any()) GroupedActivities.Add(new ActivityGroup("Today", today));
                if (yesterday.Any()) GroupedActivities.Add(new ActivityGroup("Yesterday", yesterday));
                if (thisWeek.Any()) GroupedActivities.Add(new ActivityGroup("This Week", thisWeek));
                if (lastWeek.Any()) GroupedActivities.Add(new ActivityGroup("Last Week", lastWeek));
                if (thisMonth.Any()) GroupedActivities.Add(new ActivityGroup("This Month", thisMonth));
                if (lastMonth.Any()) GroupedActivities.Add(new ActivityGroup("Last Month", lastMonth));
                if (older.Any()) GroupedActivities.Add(new ActivityGroup("Older", older));
            });
        }

        [RelayCommand]
        private void GoToDetailsAsync(Activity selectedItem)
        {
            if (selectedItem == null) return;

            // Here you can implement the logic to open a detailed view of the selected activity.
            SelectedItem = selectedItem;
            ActivityDetailsBottomSheetState = BottomSheetState.HalfExpanded;
        }

        [RelayCommand]
        private async Task EditActivityAsync(Activity selectedActivity)
        {
            if (selectedActivity == null) return;

            await Shell.Current.GoToAsync(nameof(PlacePosition), new Dictionary<string, object>
            {
                { "ActivityToEdit", selectedActivity}
            });

            ActivityDetailsBottomSheetState = BottomSheetState.Hidden;
        }

        [RelayCommand]
        private async Task DeleteActivityAsync(Activity selectedItem)
        {
            if (selectedItem == null) return;

            var confirm = await Shell.Current.DisplayAlert("Confirm Deletion",
                $"Are you sure you want to delete this activity ? This action cannot be undone.",
                "Confirm Delete", "Cancel");
            if (confirm)
            {
                // TODO: Call API to delete the activity 
                ActivityDetailsBottomSheetState = BottomSheetState.Hidden;

            }
            else
            {
                return;
            }
        }
    }
}
