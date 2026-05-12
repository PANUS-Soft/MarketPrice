using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using MarketPrice.Domain.Activity.DTOs;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Domain.Position.DTOs;
using MarketPrice.Ui.Common;
using MarketPrice.Ui.Extensions;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;
using MarketPrice.Domain.Reference.DTOs;
using MarketPrice.Ui.Models;

namespace MarketPrice.Ui.ViewModels
{
    [QueryProperty(nameof(ActivityToEdit), "ActivityToEdit")]
    [QueryProperty(nameof(PositionType), "PositionType")]
    public partial class PlacePositionViewModel : ObservableObject
    {
        private readonly ReferenceDataApiService _referenceDataApi;
        private readonly SessionService _sessionService;
        private readonly PositionApiService _positionApi;

        [ObservableProperty] private PositionStep currentStep;
        [ObservableProperty] private PositionType positionType;

        [ObservableProperty] private bool isEditMode;
        [ObservableProperty] private Guid? editingPositionId;

        // Collections used to store reference data 
        public ObservableCollection<CommodityTypeDto> CommodityTypes { get; } = new();
        public ObservableCollection<CommodityDto> Commodities { get; } = new();
        public ObservableCollection<RegionDto> Regions { get; } = new();

        // Properties used for receiving navigation parameters
        private Activity? activityToEdit;

        public Activity? ActivityToEdit
        {
            get => activityToEdit;
            set
            {
                SetProperty(ref activityToEdit, value);
                if (value != null) LoadFromActivity(value);
            }
        }

        // Properties used to bound data to the view
        [ObservableProperty] private CommodityTypeDto? selectedCommodityType;
        [ObservableProperty] private CommodityDto? selectedCommodity;
        [ObservableProperty] private RegionDto? selectedOriginRegion;
        [ObservableProperty] private RegionDto? selectedDestinationRegion;
        [ObservableProperty] private string selectedGrade;
        [ObservableProperty] private decimal quantity;
        [ObservableProperty] private string description;
        [ObservableProperty] private decimal unitPrice;
        [ObservableProperty] private bool isDeliverable;
        [ObservableProperty] private bool startPositionImmediately;
        [ObservableProperty] private DateTime? startDate;
        [ObservableProperty] private DateTime? effectiveStartDate;
        [ObservableProperty] private DateTime? endDate;
        [ObservableProperty] private string originTown;
        [ObservableProperty] private string destinationTown;
        [ObservableProperty] private string originStreet;
        [ObservableProperty] private string destinationStreet;
        [ObservableProperty] private string originQuarter;
        [ObservableProperty] private string destinationQuarter;
        [ObservableProperty] private decimal deliveryFee;
        [ObservableProperty] private string leadTime;

        // Properties for handling data validation
        [ObservableProperty] private string? commodityTypeError;
        [ObservableProperty] private string? commodityError;
        [ObservableProperty] private string? gradeError;
        [ObservableProperty] private string? quantityError;
        [ObservableProperty] private string? unitPriceError;
        [ObservableProperty] private string? startDateError;
        [ObservableProperty] private string? endDateError;
        [ObservableProperty] private string? dateError;
        [ObservableProperty] private string? originRegionError;
        [ObservableProperty] private string? originTownError;
        [ObservableProperty] private string? originQuarterError;
        [ObservableProperty] private string? originDetailError;
        [ObservableProperty] private string? destinationRegionError;
        [ObservableProperty] private string? destinationTownError;
        [ObservableProperty] private string? destinationQuarterError;
        [ObservableProperty] private string? destinationDetailError;
        [ObservableProperty] private string? leadTimeError;
        [ObservableProperty] private string? deliveryFeeError;

        // Derived and notifying properties based on UI contexts 
        public int? ShelfLifeInDays => SelectedCommodity?.ShelfLifeInDays;
        public short? LotSize => SelectedCommodity?.LotSize;
        public string? UnitOfMeasure => SelectedCommodityType?.UnitOfMeasure;
        public decimal? TotalQuantity => SelectedCommodityType == null ? 0 : LotSize * Quantity;
        public string? TotalQuantityDisplay => $"{TotalQuantity} {UnitOfMeasure}";
        public string? ShelfLifeInDaysDisplay => SelectedCommodity == null ? "" : $"Shelf Life: {ShelfLifeInDays} days";
        public string? LotSizeDisplay => SelectedCommodity == null ? "" : $"Lot Size: {LotSize} {UnitOfMeasure}";
        public string? TotalValueDisplay => SelectedCommodity == null || UnitPrice <= 0 ? "" : PositionType == PositionType.Bid ? $"Total Bid Value: {UnitPrice * Quantity} FCFA" : $"Total Offer Value: {UnitPrice * Quantity} FCFA";

        // Properties handling UI visibility (state indicators)
        public bool IsCommodityDetailsStep => CurrentStep == PositionStep.CommodityDetails;
        public bool IsPricingAndTimingStep => CurrentStep == PositionStep.PricingInformation;
        public bool IsLogisticsInformationStep => CurrentStep == PositionStep.LogisticsInformation;
        public bool IsBackTextVisible => CurrentStep >= PositionStep.PricingInformation;
        public string CurrentStepDisplay => CurrentStep.GetDisplayName();
        public bool IsOffer => PositionType == PositionType.Offer;
        public bool IsCommodityTypeEditable => IsCommodityDetailsStep;
        public bool IsCommodityEditable => SelectedCommodityType != null;
        public bool IsStartDateTimeEnabled => !IsEditMode && !StartPositionImmediately;
        public bool IsStartPositionImmediatelyEnabled => !IsEditMode;
        public string PageTitle => IsEditMode ? $"Edit {ActivityToEdit!.PosType}" : PositionType == PositionType.Bid ? "Place a Bid" : "Place an Offer";
        public string ContinueButtonText => CurrentStep == PositionStep.LogisticsInformation ? (PositionType == PositionType.Bid ? "Place a Bid" : "Place an Offer") : "Continue";
        
        private readonly Color _activeColor = Color.FromArgb("#0056A0");
        private readonly Color _inactiveColor = Color.FromArgb("#D1D5DB");

        public Color Step1Color => _activeColor;
        public Color Step2Color => CurrentStep >= PositionStep.PricingInformation ? _activeColor : _inactiveColor;
        public Color Step3Color => CurrentStep == PositionStep.LogisticsInformation ? _activeColor : _inactiveColor;
        
        public PlacePositionViewModel(ReferenceDataApiService referenceDataApiService, SessionService sessionService, PositionApiService positionApiService)
        {
            CurrentStep = PositionStep.CommodityDetails;
            _referenceDataApi = referenceDataApiService;
            _sessionService = sessionService;
            _positionApi = positionApiService;
        }

        partial void OnPositionTypeChanged(PositionType value)
        {
            OnPropertyChanged(nameof(PageTitle));
            OnPropertyChanged(nameof(IsOffer));
            OnPropertyChanged(nameof(IsDeliverable));
        }

        partial void OnCurrentStepChanged(PositionStep value)
        {
            OnPropertyChanged(nameof(IsCommodityDetailsStep));
            OnPropertyChanged(nameof(IsPricingAndTimingStep));
            OnPropertyChanged(nameof(IsLogisticsInformationStep));
            OnPropertyChanged(nameof(IsBackTextVisible));
            OnPropertyChanged(nameof(CurrentStepDisplay));
            OnPropertyChanged(nameof(ContinueButtonText));
            OnPropertyChanged(nameof(Step1Color));
            OnPropertyChanged(nameof(Step2Color));
            OnPropertyChanged(nameof(Step3Color));
            OnPropertyChanged(nameof(IsCommodityTypeEditable));
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
        private async Task BackToMarketInsightAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private void SelectGrade(string grade)
        {
            SelectedGrade = grade;
        }

        [RelayCommand]
        private async Task ContinueAsync()
        {
            if (IsCommodityDetailsStep)
            {
                bool isValid = ValidateCommodityDetails();
                if (!isValid) return;
            }

            else if (IsPricingAndTimingStep)
            {
                bool isValid = ValidatePricingAndTiming();
                if (!isValid) return;
            }

            else
            {
                bool isValid = ValidateLogistics();
                if (!isValid) return;

                if (PositionType == PositionType.Bid)
                {
                    await CreateBidAsync();
                }
                else
                {
                    await CreateOfferAsync();
                }
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

        // Loading the reference data to be used during position placement
        public async Task LoadReferenceDataAsync()
        {
            var isSessionValid = await _sessionService.ValidateAndRefreshSessionAsync();

            if (isSessionValid) await _sessionService.GetCurrentSessionAsync();
            else await _sessionService.TryRefreshTokenAsync();
            try
            {
                // Logic to load reference data for CommodityTypes and Regions
                var commodityTypesResponse = await _referenceDataApi.GetCommodityTypesAsync();
                var commoditiesResponse = await _referenceDataApi.GetCommoditiesAsync();
                var regionsResponse = await _referenceDataApi.GetRegionsAsync();

                if (commodityTypesResponse.IsSuccessStatusCode)
                {
                    CommodityTypes.Clear();
                    var commodityTypes = await commodityTypesResponse.Content.ReadFromJsonAsync<List<CommodityTypeDto>>();
                    if (commodityTypes != null)
                        foreach (var ct in commodityTypes) CommodityTypes.Add(ct);
                }

                if (commoditiesResponse.IsSuccessStatusCode)
                {
                    Commodities.Clear();
                    var commodities = await commoditiesResponse.Content.ReadFromJsonAsync<List<CommodityDto>>();
                    if (commodities != null)
                        foreach (var c in commodities) Commodities.Add(c);
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

        [RelayCommand]
        private async Task LoadCommoditiesByCommodityTypesAsync(Guid id)
        {
            var isSessionValid = await _sessionService.ValidateAndRefreshSessionAsync();

            if (isSessionValid) await _sessionService.GetCurrentSessionAsync();
            else await _sessionService.TryRefreshTokenAsync();

            try
            {
                // Logic to load reference data for Commodities based on the selected commodity type
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


        // Methods notifying property changes
        partial void OnSelectedCommodityTypeChanged(CommodityTypeDto? value)
        {
            if (value != null) CommodityTypeError = null;
            SelectedCommodity = null;
            Commodities.Clear();
            if (value == null)
                return;

            LoadCommoditiesByCommodityTypesCommand.Execute(value.Id);

            OnPropertyChanged(nameof(UnitOfMeasure));
            OnPropertyChanged(nameof(IsCommodityEditable));
        }

        partial void OnSelectedCommodityChanged(CommodityDto? value)
        {
            if (value != null) CommodityError = null;

            OnPropertyChanged(nameof(LotSize));
            OnPropertyChanged(nameof(ShelfLifeInDays));
            OnPropertyChanged(nameof(TotalQuantityDisplay));
            OnPropertyChanged(nameof(LotSizeDisplay));
            OnPropertyChanged(nameof(ShelfLifeInDaysDisplay));
        }

        partial void OnIsEditModeChanged(bool value)
        {
            OnPropertyChanged(nameof(PageTitle));
        }

        

        partial void OnSelectedGradeChanged(string value)
        {
            if (!string.IsNullOrEmpty(value)) GradeError = null;
        }

        partial void OnQuantityChanged(decimal value)
        {
            if (value > 0) QuantityError = null;
            OnPropertyChanged(nameof(TotalQuantityDisplay));
        }

        partial void OnUnitPriceChanged(decimal value)
        {
            if (value > 0) UnitPriceError = null;
            OnPropertyChanged(nameof(TotalValueDisplay));
        }

        partial void OnStartPositionImmediatelyChanged(bool value)
        {
            if (value)
            {
                StartDate = DateTime.Now;
                StartDateError = null;
            }
            CombineDateError();
            OnPropertyChanged(nameof(IsStartDateTimeEnabled));
        }

        partial void OnStartDateChanged(DateTime? value)
        {
            if (IsEditMode) StartDateError = null;

            if (!IsEditMode && StartPositionImmediately == false && value <= DateTime.Now) StartDateError = "- Start date cannot be in the past. \n";

            if (StartPositionImmediately == false && value != null && value >= DateTime.Now) StartDateError = null;
            CombineDateError();
            OnPropertyChanged(nameof(EndDateError));
            OnPropertyChanged(nameof(DateError));
        }

        partial void OnEndDateChanged(DateTime? value)
        {
            if (value < StartDate) EndDateError = "- End date cannot be less than the start date.\n";

            else if (value <= DateTime.Now) EndDateError = "- End date must be in the future.\n";

            if (value == StartDate) EndDateError = "- End date cannot be equal to the start date.\n";

            if (value != null && value > StartDate) EndDateError = null;
            CombineDateError();
            OnPropertyChanged(nameof(StartDateError));
            OnPropertyChanged(nameof(DateError));
        }

        partial void OnSelectedOriginRegionChanged(RegionDto? value)
        {
            if (value != null) OriginRegionError = null;
            CombineOriginDetailError();
            OnPropertyChanged(nameof(OriginDetailError));
        }

        partial void OnOriginTownChanged(string value)
        {
            if (!string.IsNullOrEmpty(value)) OriginTownError = null;
            CombineOriginDetailError();
            OnPropertyChanged(nameof(OriginDetailError));
        }

        partial void OnOriginQuarterChanged(string value)
        {
            if (!string.IsNullOrEmpty(value)) OriginQuarterError = null;
            CombineOriginDetailError();
            OnPropertyChanged(nameof(OriginDetailError));
        }

        partial void OnSelectedDestinationRegionChanged(RegionDto? value)
        {
            if (value != null) DestinationRegionError = null;
            CombineDestinationDetailError();
            OnPropertyChanged(nameof(DestinationDetailError));
        }

        partial void OnDestinationTownChanged(string value)
        {
            if (!string.IsNullOrEmpty(value)) DestinationTownError = null;
            CombineDestinationDetailError();
            OnPropertyChanged(nameof(DestinationDetailError));
        }

        partial void OnDestinationQuarterChanged(string value)
        {
            if (!string.IsNullOrEmpty(value)) DestinationQuarterError = null;
            CombineDestinationDetailError();
            OnPropertyChanged(nameof(DestinationDetailError));
        }

        partial void OnDeliveryFeeChanged(decimal value)
        {
            if (value > 0) DeliveryFeeError = null;
        }

        partial void OnLeadTimeChanged(string value)
        {
            if (!string.IsNullOrEmpty(value)) LeadTimeError = null;
        }

        // Helper methods for live data validation

        private void CombineDateError()
        {
            DateError = StartDateError + EndDateError;
        }

        private void CombineOriginDetailError()
        {
            OriginDetailError = OriginRegionError + OriginTownError + OriginQuarterError;
        }

        private void CombineDestinationDetailError()
        {
            DestinationDetailError = DestinationRegionError + DestinationTownError + DestinationQuarterError;
        }

        // Data Validation Methods
        public bool ValidateCommodityDetails()
        {
            bool isValid = true;

            CommodityTypeError = null;
            CommodityError = null;
            QuantityError = null;
            GradeError = null;

            if (SelectedCommodityType == null)
            {
                CommodityTypeError = "Please select a commodity type";
                isValid = false;
            }

            if (SelectedCommodity == null)
            {
                CommodityError = "Please select a commodity";
                isValid = false;
            }

            if (Quantity <= 0)
            {
                QuantityError = "Quantity must be greater than 0";
                isValid = false;
            }

            if (string.IsNullOrEmpty(SelectedGrade))
            {
                GradeError = "Please select a grade";
                isValid = false;
            }

            return isValid;
        }

        private bool ValidatePricingAndTiming()
        {
            bool isValid = true;

            UnitPriceError = null;
            StartDateError = null;
            EndDateError = null;

            if (UnitPrice <= 0)
            {
                UnitPriceError = "Unit Price should be greater than 0";
                isValid = false;
            }

            DateTime? effectiveStart = StartPositionImmediately ? DateTime.Now : StartDate ?? null;

            if (!StartPositionImmediately && StartDate == null)
            {
                StartDateError = "- Start Date is required.\n";
                isValid = false;
            }
            
            if (!IsEditMode && !StartPositionImmediately && StartDate < DateTime.Now)
            {
                StartDateError = "- Start of the position cannot be in the past.\n";
                isValid = false;
            }

            if (EndDate == null)
            {
                EndDateError = "- End Date is required.\n";
                isValid = false;
            }
            else if (EndDate <= DateTime.Now)
            {
                EndDateError = "- End date must be in the future.\n";
                isValid = false;
            }
            else if (EndDate <= effectiveStart)
            {
                EndDateError = "- End date must be after start date.\n";
                isValid = false;
            }

            if (StartDate != null && EndDate != null && EndDate == effectiveStart)
            {
                EndDateError = "- End date can not be equal to the start date.\n";
            }

            CombineDateError();
            return isValid;
        }


        private bool ValidateLogistics()
        {
            bool isValid = true;

            OriginRegionError = null;
            OriginTownError = null;
            OriginQuarterError = null;
            OriginDetailError = null;

            if (SelectedOriginRegion == null)
            {
                OriginRegionError = "- Please select a region.\n";
                OriginDetailError += OriginRegionError;
                isValid = false;
            }

            if (string.IsNullOrEmpty(OriginTown))
            {
                OriginTownError = "- Town is required.\n";
                OriginDetailError += OriginTownError;
                isValid = false;
            }

            if (string.IsNullOrEmpty(OriginQuarter))
            {
                OriginQuarterError = "- Quarter is required.\n";
                OriginDetailError += OriginQuarterError;
                isValid = false;
            }

            if (IsDeliverable)
            {
                DestinationRegionError = null;
                DestinationTownError = null;
                DestinationQuarterError = null;
                DestinationDetailError = null;
                DeliveryFeeError = null;
                LeadTimeError = null;

                if (SelectedDestinationRegion == null)
                {
                    DestinationRegionError = "- Please select a destination region.\n";
                    DestinationDetailError += DestinationRegionError;
                    isValid = false;
                }

                if (string.IsNullOrEmpty(DestinationTown))
                {
                    DestinationTownError = "- Destination town is required.\n";
                    DestinationDetailError += DestinationTownError;
                    isValid = false;
                }

                if (string.IsNullOrEmpty(DestinationQuarter))
                {
                    DestinationQuarterError = "- Destination quarter is required.\n";
                    DestinationDetailError += DestinationQuarterError;
                    isValid = false;
                }
                
                if (DeliveryFee <= 0)
                {
                    DeliveryFeeError = "Delivery Fee should be greater than 0";
                    isValid = false;
                }

                if (string.IsNullOrEmpty(LeadTime))
                {
                    LeadTimeError = "Lead time should not be empty";
                    isValid = false;
                }
            }

            return isValid;
        }

        // Method to prefill data in edit mode - to be called when editing an existing position (bid or offer)
        public async Task LoadFromActivity(Activity activity)
        {

            IsEditMode = true;
            EditingPositionId = activity.PositionId;

            await LoadReferenceDataAsync();

            // Commodity details
            SelectedCommodityType = CommodityTypes.FirstOrDefault(ct => ct.Id == activity.CommodityTypeId);

            if (SelectedCommodityType != null)
            {
                await LoadCommoditiesByCommodityTypesAsync(SelectedCommodityType.Id);

                SelectedCommodity = Commodities
                    .FirstOrDefault(c => c.Id == activity.CommodityId);
            }

            //SelectedCommodity = Commodities.FirstOrDefault(c => c.Id == activity.CommodityId);
            SelectedGrade = activity.Grade;
            Quantity = decimal.Parse(activity.Quantity.Replace(",", ""));
            Description = activity.Description;

            // Pricing and timing details
            UnitPrice = decimal.Parse(activity.Price.Replace(" FCFA", ""));
            StartPositionImmediately = false;
            StartDate = activity.StartDate.DateTime;
            EndDate = activity.EndDate.DateTime;

            // Logistics Details
            SelectedOriginRegion = Regions.FirstOrDefault(r => r.Id == activity.Origin?.RegionId);
            OriginTown = activity.Origin?.Town;
            OriginQuarter = activity.Origin?.Quarter;
            OriginStreet = activity.Origin?.Street;
            
            IsDeliverable = activity.IsDeliverable;

            if (activity.IsDeliverable)
            {
                IsDeliverable = true;
                DeliveryFee = decimal.Parse(activity.DeliveryFee.Replace(" FCFA", ""));
                LeadTime = activity.LeadTime ?? "";
                SelectedDestinationRegion = Regions.FirstOrDefault(r => r.Id == activity.Destination?.RegionId);
                DestinationTown = activity.Destination?.Town ?? "";
                DestinationQuarter = activity.Destination?.Quarter ?? "";
                DestinationStreet = activity.Destination?.Street ?? "";
            }


        }

        // Methods to create bids and offers
        private async Task CreateBidAsync()
        {
            try
            {
                // Logic to create a bid
                var isSessionValid = await _sessionService.ValidateAndRefreshSessionAsync();

                if (isSessionValid) await _sessionService.GetCurrentSessionAsync();
                else await _sessionService.TryRefreshTokenAsync();
                
                var userSession = await _sessionService.GetCurrentSessionAsync();
                var command = new CreatePositionCommand
                {
                    UserId = userSession!.UserId,
                    CommodityId = SelectedCommodity!.Id,
                    UnitPrice = (decimal)UnitPrice!,
                    Quantity = (decimal)Quantity!,
                    Grade = SelectedGrade,
                    Description = Description,
                    StartDate = StartPositionImmediately ? DateTime.Now : (DateTime)StartDate!,
                    EndDate = (DateTime)EndDate!,
                    Origin = new LocationCommand
                    {
                        RegionId = SelectedOriginRegion!.Id,
                        Town = OriginTown,
                        Quarter = OriginQuarter,
                        Street = OriginStreet
                    }
                };

                DateTime effectiveStartDate = StartPositionImmediately ? DateTime.Now : (DateTime)StartDate!;

                string formattedStartDate = StartPositionImmediately ? "Now" : effectiveStartDate.ToString("g");

                var message = $""""
                               Commodity: {SelectedCommodity.Name}
                               Total Quantity: {TotalQuantityDisplay}
                               Unit Price: {(decimal)UnitPrice!} FCFA per {LotSize} {UnitOfMeasure}
                               {TotalValueDisplay}
                               Start date/time: {formattedStartDate}
                               End date/time: {EndDate:g}
                               Origin: {SelectedOriginRegion.NameInEnglish}, {OriginTown}, {OriginQuarter}
                               """";

                // Call to API to create the place bid
                var confirmBidPlacement = await Shell.Current.DisplayAlert("Confirm Bid Placement", message, "Post Bid", "Cancel");

                if (!confirmBidPlacement) return;

                var response = await _positionApi.CreateBidAsync(command);

                if (response.IsSuccessStatusCode)
                {
                    var dto = await response.Content.ReadFromJsonAsync<PositionResponseDto>();
                    if (dto != null)
                    {
                        await Toast.Make("Bid placed successfully", ToastDuration.Long).Show();
                        await Shell.Current.GoToAsync("//Market");
                    }
                } else
                {
                    await Shell.Current.DisplayAlert("Bid Placement Failed", "There was an error while placing your bid. Please try later.", "OK");
                }
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", e.Message, "OK");
            }
        }

        private async Task CreateOfferAsync()
        {
            try
            {
                // Logic to create an offer
                var isSessionValid = await _sessionService.ValidateAndRefreshSessionAsync();

                if (isSessionValid) await _sessionService.GetCurrentSessionAsync();
                else await _sessionService.TryRefreshTokenAsync();

                var userSession = await _sessionService.GetCurrentSessionAsync();
                var command = new CreatePositionCommand
                {
                    UserId = userSession!.UserId,
                    CommodityId = SelectedCommodity!.Id,
                    UnitPrice = (decimal)UnitPrice!,
                    Quantity = (decimal)Quantity!,
                    Grade = SelectedGrade,
                    Description = Description,
                    StartDate = StartPositionImmediately ? DateTime.Now : (DateTime)StartDate!,
                    EndDate = (DateTime)EndDate!,
                    CanDeliver = IsDeliverable,
                    LeadTime = IsDeliverable ? LeadTime : null,
                    DeliveryFee = IsDeliverable ? DeliveryFee : null,
                    Origin = new LocationCommand
                    {
                        RegionId = SelectedOriginRegion!.Id,
                        Town = OriginTown,
                        Quarter = OriginQuarter,
                        Street = OriginStreet
                    },
                    Destination = IsDeliverable ? new LocationCommand
                    {
                        RegionId = SelectedDestinationRegion!.Id,
                        Town = DestinationTown,
                        Quarter = DestinationQuarter,
                        Street = DestinationStreet
                    } : null,
                };

                DateTime effectiveStartDate = StartPositionImmediately ? DateTime.Now : (DateTime)StartDate!;

                string formattedStartDate = StartPositionImmediately ? "Now" : effectiveStartDate.ToString("g");


                var message = $""""
                               Commodity: {SelectedCommodity.Name}
                               Total Quantity: {TotalQuantityDisplay}
                               Unit Price: {(decimal)UnitPrice!} FCFA per {LotSize} {UnitOfMeasure}
                               {TotalValueDisplay}
                               Start date/time: {formattedStartDate}
                               End date/time: {EndDate:g}
                               Origin: {SelectedOriginRegion.NameInEnglish}, {OriginTown}, {OriginQuarter}
                               """";

                var deliveryDetails = IsDeliverable ? $""""
                                       Destination: {SelectedDestinationRegion!.NameInEnglish}, {DestinationTown}, {DestinationQuarter}
                                       Delivery Fee: {DeliveryFee} FCFA
                                       Lead Time: {LeadTime} days
                                       """" : null;

                if (IsDeliverable) message = message + "\n" + deliveryDetails;

                // Call to API to create the place offer
                var confirmOfferPlacement = await Shell.Current.DisplayAlert("Confirm Offer Placement", message, "Post Offer", "Cancel");

                if (!confirmOfferPlacement) return;

                var response = await _positionApi.CreateOfferAsync(command);

                if (response.IsSuccessStatusCode)
                {
                    var dto = await response.Content.ReadFromJsonAsync<PositionResponseDto>();
                    if (dto != null)
                    {
                        await Toast.Make("Offer placed successfully", ToastDuration.Long).Show();
                        await Shell.Current.GoToAsync("//Market");
                    }
                } else
                {
                    await Shell.Current.DisplayAlert("Offer Placement Failed", "There was an error while placing your offer. Please try later.", "OK");
                }
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error", $"There was an error while placing your offer. {e.Message}", "OK");
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
