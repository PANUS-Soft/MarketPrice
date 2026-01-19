using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Domain.Reference;
using MarketPrice.Ui.Extensions;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Services.Api;

namespace MarketPrice.Ui.ViewModels
{
    public partial class PositionViewModel : ObservableObject
    {
        private readonly ReferenceDataApiService _referenceDataApi;

        public CommodityDetails CommodityDetail { get; } = new();
        public PricingInformation PricingInfo { get; } = new();
        public LogisticsInformation LogisticsInfo { get; } = new();

        [ObservableProperty]
        private PositionStep currentStep;

        public ObservableCollection<CommodityTypeDto> CommodityTypes { get; } = new();
        public ObservableCollection<CommodityDto> Commodities { get; } = new();
        public ObservableCollection<RegionDto> Regions { get; } = new();

        [ObservableProperty]
        private CommodityTypeDto? selectedCommodityType;

        [ObservableProperty]
        private CommodityDto? selectedCommodity;

        [ObservableProperty]
        private RegionDto? selectedRegion;

        [ObservableProperty] 
        private decimal quantity;

        [ObservableProperty]
        private decimal unitPrice;

        public int? ShelfLifeInDays => SelectedCommodity?.ShelfLifeInDays;
        public short? LotSize => SelectedCommodity?.LotSize;
        public string? UnitOfMeasure => SelectedCommodityType?.UnitOfMeasure;

        public decimal? TotalQuantity => SelectedCommodityType == null ? 0 : LotSize * Quantity;
        public string? TotalQuantityDisplay => $"{TotalQuantity} {UnitOfMeasure}";
        public string? ShelfLifeInDaysDisplay => SelectedCommodity == null ? "" : $"Shelf Life: {ShelfLifeInDays} days";
        public string? LotSizeDisplay => SelectedCommodity == null ? "" : $"Lot Size: {LotSize} {UnitOfMeasure}";
        public string? OfferTotalValue => SelectedCommodity == null && UnitPrice == null ? "" : $"Total Offer Value: {LotSize * UnitPrice} FCFA";
        public string? BidTotalValue => SelectedCommodity == null && UnitPrice == null ? "" : $"Total Bid Value: {LotSize * UnitPrice} FCFA";

        public bool IsCommodityDetailsStep => CurrentStep == PositionStep.CommodityDetails;
        public bool IsPricingAndTimingStep => CurrentStep == PositionStep.PricingInformation;
        public bool IsLogisticsInformationStep => CurrentStep == PositionStep.LogisticsInformation;
        public bool IsBackTextVisible => CurrentStep >= PositionStep.PricingInformation;
        public string CurrentStepDisplay => CurrentStep.GetDisplayName();


        public event Func<Task<bool>>? ValidateCurrentStepRequested;
        public string BidButtonText => CurrentStep == PositionStep.LogisticsInformation ? "Place a Bid" : "Continue";
        public string OfferButtonText => CurrentStep == PositionStep.LogisticsInformation ? "Place an Offer" : "Continue";

        private readonly Color _activeColor = Color.FromArgb("#0056A0");
        private readonly Color _inactiveColor = Color.FromArgb("#D1D5DB");

        public Color Step1Color => _activeColor;
        public Color Step2Color => CurrentStep >= PositionStep.PricingInformation ? _activeColor : _inactiveColor;
        public Color Step3Color => CurrentStep == PositionStep.LogisticsInformation ? _activeColor : _inactiveColor;
        public PositionViewModel(ReferenceDataApiService referenceDataApiService)
        {
            CurrentStep = PositionStep.CommodityDetails;
            _referenceDataApi = referenceDataApiService;
        }

        partial void OnCurrentStepChanged(PositionStep value)
        {
            if (value == PositionStep.CommodityDetails)
            {
                LoadReferenceDataCommand.Execute(null);
            }

            OnPropertyChanged(nameof(IsCommodityDetailsStep));
            OnPropertyChanged(nameof(IsPricingAndTimingStep));
            OnPropertyChanged(nameof(IsLogisticsInformationStep));
            OnPropertyChanged(nameof(IsBackTextVisible));
            OnPropertyChanged(nameof(CurrentStepDisplay));
            OnPropertyChanged(nameof(BidButtonText));
            OnPropertyChanged(nameof(OfferButtonText));
            OnPropertyChanged(nameof(Step1Color));
            OnPropertyChanged(nameof(Step2Color));
            OnPropertyChanged(nameof(Step3Color));
        }

        [RelayCommand]
        private void Back()
        {
            if (CurrentStep == PositionStep.LogisticsInformation)
                CurrentStep = PositionStep.PricingInformation;
            else if (CurrentStep == PositionStep.PricingInformation)
                CurrentStep = PositionStep.CommodityDetails;
        }

        [RelayCommand]
        private void BackToMarket()
        {
            Shell.Current.GoToAsync("Market");
        }

        [RelayCommand]
        private async Task BidContinueAsync()
        {
            if (ValidateCurrentStepRequested != null)
            {
                bool isValid = await ValidateCurrentStepRequested.Invoke();
                if (!isValid)
                    return;
            }

            if (CurrentStep == PositionStep.LogisticsInformation)
            {
                await CreateBidAsync();
                return;
            }

            MoveToNextStep();
        }

        [RelayCommand]
        private async Task OfferContinueAsync()
        {
            if (ValidateCurrentStepRequested != null)
            {
                bool isValid = await ValidateCurrentStepRequested.Invoke();
                if (!isValid)
                    return;
            }
            if (CurrentStep == PositionStep.LogisticsInformation)
            {
                await CreateOfferAsync();
                return;
            }
            MoveToNextStep();
        }

        private void MoveToNextStep()
        {
            if (CurrentStep == PositionStep.CommodityDetails)
                CurrentStep = PositionStep.PricingInformation;
            else if (CurrentStep == PositionStep.PricingInformation)
                CurrentStep = PositionStep.LogisticsInformation;
        }

        [RelayCommand]
        private async Task LoadReferenceDataAsync()
        {
            try
            {
                // Logic to load reference data for CommodityTypes and Regions
                var commodityTypesResponse = await _referenceDataApi.GetCommodityTypesAsync();
                var regionsResponse = await _referenceDataApi.GetRegionsAsync();

                if (commodityTypesResponse.IsSuccessStatusCode)
                {
                    CommodityTypes.Clear();
                    var commodityTypes = await commodityTypesResponse.Content.ReadFromJsonAsync<List<CommodityTypeDto>>();
                    if (commodityTypes != null)
                        foreach (var ct in commodityTypes) CommodityTypes.Add(ct);
                }

                if (regionsResponse.IsSuccessStatusCode)
                {
                    Regions.Clear();
                    var regions = await regionsResponse.Content.ReadFromJsonAsync<List<RegionDto>>();
                    if (regions != null)
                        foreach (var region in regions) Regions.Add(region);
                }
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", $"There was an error while loading reference data. {e.Message}.", "OK");
            }
        }

        partial void OnQuantityChanged(decimal value)
        {
            OnPropertyChanged(nameof(TotalQuantityDisplay));
        }

        partial void OnUnitPriceChanged(decimal value)
        {
            OnPropertyChanged(nameof(OfferTotalValue));
            OnPropertyChanged(nameof(BidTotalValue));
        }

        

        partial void OnSelectedCommodityTypeChanged(CommodityTypeDto? value)
        {
            SelectedCommodity = null;
            Commodities.Clear();
            if (value == null)
                return;
            
            LoadCommoditiesByCommodityTypesCommand.Execute(value.Id);

            OnPropertyChanged(nameof(UnitOfMeasure));
        }

        partial void OnSelectedCommodityChanged(CommodityDto? value)
        {
            OnPropertyChanged(nameof(LotSize));
            OnPropertyChanged(nameof(ShelfLifeInDays));
            OnPropertyChanged(nameof(TotalQuantityDisplay));
            OnPropertyChanged(nameof(LotSizeDisplay));
            OnPropertyChanged(nameof(ShelfLifeInDaysDisplay));
        }

        [RelayCommand]
        private async Task LoadCommoditiesByCommodityTypesAsync(Guid id)
        {
            try
            {
                var commoditiesResponse = await _referenceDataApi.GetCommoditiesByCommodityTypeIdAsync(id);
                if (commoditiesResponse.IsSuccessStatusCode)
                {
                    Commodities.Clear();
                    var commodities = await commoditiesResponse.Content.ReadFromJsonAsync<List<CommodityDto>>();
                    if (commodities != null)
                        foreach (var c in commodities) Commodities.Add(c);
                }
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", $"There was an error while loading commodities. {e.Message}.", "OK");
            }
        }

        private async Task CreateBidAsync()
        {
            try
            {
                // Logic to create a bid
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", $"There was an error while placing your bid. {e.Message}.", "OK");
            }
        }

        private async Task CreateOfferAsync()
        {
            try
            {
                // Logic to create an offer
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", $"There was an error while placing your offer. {e.Message}.", "OK");
            }
        }
    }

    public enum PositionStep
    {
        [Display(Name = "Step 1 of 3: Commodity Details")]
        CommodityDetails,

        [Display(Name = "Step 2 of 3: Pricing and Timing")]
        PricingInformation,

        [Display(Name = "Step 3 of 3: Logistics")]
        LogisticsInformation
    }
}
