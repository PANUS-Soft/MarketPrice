using System.Collections.ObjectModel;
using System.Net.Http.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Domain.Position.DTOs;
using MarketPrice.Ui.Common;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;
using Microsoft.Extensions.Options;

namespace MarketPrice.Ui.ViewModels
{
    public partial class PositionListingViewModel : ObservableObject
    {
        private readonly SessionService _sessionService;
        private readonly PositionApiService _positionApi;
        private readonly ApiSettings _apiSettings;

        private PositionListingCommand? _navigationArgs;

        public ObservableCollection<FilterChip> CommodityFilters { get; } = new();
        public ObservableCollection<PositionListing> Listings { get; } = new();
        public ObservableCollection<PositionListing> Positions { get; } = new ();

        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private bool isListEmpty;
        [ObservableProperty] private string commodityTypeName;
        [ObservableProperty] private string positionTypeName;
        [ObservableProperty] private decimal unitPrice;
        [ObservableProperty] private string lotSize;
        [ObservableProperty] private string priceDisplay;
        [ObservableProperty] private string selectedCommodity;

        public PositionListingViewModel(SessionService sessionService, PositionApiService positionApi, IOptions<ApiSettings> apiSettings)
        {
            _sessionService = sessionService;
            _positionApi = positionApi;
            _apiSettings = apiSettings.Value;
        }

        public PositionListingViewModel() { }

        public async Task InitializeAsync(PositionListingCommand args)
        {
            _navigationArgs = args;

            SelectedCommodity = args.CommodityId == null ? "ALL" : args.CommodityName?.ToUpper();

            await EnsureSessionActiveAsync();
            await LoadPositionListingAsync();
        }

        private async Task EnsureSessionActiveAsync()
        {
            var isSessionValid = await _sessionService.ValidateAndRefreshSessionAsync();

            if (isSessionValid) await _sessionService.GetCurrentSessionAsync();
            else await _sessionService.TryRefreshTokenAsync();
        }

        private async Task LoadPositionListingAsync()
        {
            if (_navigationArgs == null) return;

            IsBusy = true;

            var command = new PositionListingCommand
            {
                CommodityTypeId = _navigationArgs.CommodityTypeId,
                CommodityId = null,
                PositionTypeId = _navigationArgs.PositionTypeId,
                UnitPrice = _navigationArgs.UnitPrice
            };

            var positionListingResponse = await _positionApi.GetPositionListingAsync(command);

            if (!positionListingResponse.IsSuccessStatusCode)
            {
                IsBusy = false;
                return;
            }

            var dto = await positionListingResponse.Content.ReadFromJsonAsync<PositionListingResponseDto>();

            if (dto == null)
            {
                IsBusy = false;
                return;
            }

            CommodityTypeName = dto.CommodityTypeName?.ToUpper() ?? string.Empty;
            LotSize = dto.LotSize;
            UnitPrice = dto.UnitPrice;
            PositionTypeName = $"Position Listings - {dto.PositionTypeName?.ToUpper()}" ?? string.Empty;

            CommodityFilters.Clear();
            CommodityFilters.Add(new FilterChip { Title = "ALL" });

            foreach (var name in dto.CommodityNames)
            {
                CommodityFilters.Add(new FilterChip { Title = name.ToUpper() });
            }

            Listings.Clear();
            foreach (var item in dto.Listings) Listings.Add(item);

            foreach(var item in Listings) Positions.Add(item);

            PriceDisplay = $"{UnitPrice} FCFA per {LotSize}";

            IsBusy = false;

            var selectedChip = CommodityFilters.FirstOrDefault(c => c.Title == SelectedCommodity);

            SelectFilter(selectedChip ?? CommodityFilters.First());
        }

        private Guid? ResolveCommodityId()
        {
            if (SelectedCommodity == "ALL") return null;

            return _navigationArgs?.CommodityId;
        }

        [RelayCommand]
        private void SelectFilter(FilterChip selectedChip)
        {
            if (selectedChip == null) return;

            SelectedCommodity = selectedChip.Title.ToUpper();

            // Visual update (Radio button style)
            foreach (var chip in CommodityFilters)
            {
                chip.IsSelected = chip.Title == selectedChip.Title;
            }

            Positions.Clear();

            if (selectedChip.Title == "ALL")
            {
                foreach (var item in Listings) Positions.Add(item);
            }
            else
            {
                var filtered = Listings
                    .Where(p => p.CommodityName!.Equals(selectedChip.Title, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var item in filtered) Positions.Add(item);
            }
            
            IsListEmpty = Positions.Count == 0;
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task GoToPositionDetailsAsync(PositionListing item)
        {
            await Shell.Current.GoToAsync("PositionDetail");
        }
    }
}