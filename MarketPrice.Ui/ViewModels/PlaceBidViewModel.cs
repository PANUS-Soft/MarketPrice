using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Domain.Reference;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;

namespace MarketPrice.Ui.ViewModels
{
    public partial class PlaceBidViewModel (ReferenceDataApiService referenceDataApi, SessionService sessionService) : ObservableValidator
    {
        public ObservableCollection<RegionDto> Regions { get; } = new();
        public ObservableCollection<CommodityTypeDto> CommodityTypes { get; } = new();
        public ObservableCollection<CommodityDto> Commodities { get; } = new();


        [ObservableProperty] 
        [Required(ErrorMessage = "Grade is required.")]
        private string _selectedGrade;

        [ObservableProperty]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater then zero.")]
        [NotifyPropertyChangedFor(nameof(TotalBidValue))]
        private int _quantity;

        [ObservableProperty] private string _unitOfMeasure;

        [ObservableProperty]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit Price must be greater then zero.")]
        [NotifyPropertyChangedFor(nameof(TotalBidValue))]
        private decimal _unitPrice;

        [ObservableProperty] 
        private string _description;

        [ObservableProperty]
        [Required(ErrorMessage = "Start Date is required.")]
        [NotifyPropertyChangedFor(nameof(EndDate))]
        private DateTime? _startDate;
        
        [ObservableProperty]
        [Required(ErrorMessage = "End Date is required.")]
        [CustomValidation(typeof(PlaceBidViewModel), nameof(ValidateEndDate))]
        private DateTime? _endDate;

        public static ValidationResult? ValidateEndDate(DateTime? endDate, ValidationContext context)
        {
            // We get access to the whole ViewModel instance here
            var instance = (PlaceBidViewModel)context.ObjectInstance;

            // If either date is missing, let the [Required] attribute handle that error instead
            if (endDate == null || instance.StartDate == null)
            {
                return ValidationResult.Success;
            }

            // The actual logic: End Date must be greater than Start Date
            if (endDate <= instance.StartDate)
            {
                return new ValidationResult("End date must be later than the start date.");
            }

            return ValidationResult.Success;
        }

        [ObservableProperty]
        [Required(ErrorMessage = "Town is required.")]
        private string _town;
        
        [ObservableProperty]
        [Required(ErrorMessage = "Quarter is required.")]
        private string _quarter;

        [ObservableProperty]
        [Required(ErrorMessage = "Street is required.")]
        private string _street;

        [ObservableProperty]
        private int _shelfLifeInDays;

        [ObservableProperty]
        private decimal _lotSize;

        public decimal TotalBidValue => UnitPrice * Quantity;

        [ObservableProperty] 
        private CommodityTypeDto _selectedCommodityType;

        [ObservableProperty]
        [Required(ErrorMessage = "Commodity is required.")]
        private CommodityDto _selectedCommodity;

        [ObservableProperty]
        [Required(ErrorMessage = "Region is required.")]
        private RegionDto _selectedRegion;
        async partial void OnSelectedCommodityTypeChanged(CommodityTypeDto value)
        {
            await LoadCommoditiesForSelectedType();
        }

        private async Task LoadInitialDataAsync()
        {
            var regionsResponse = await referenceDataApi.GetRegionsAsync();
            var commodityTypesResponse= await referenceDataApi.GetCommodityTypesAsync();

            if (regionsResponse.IsSuccessStatusCode)
            {
                var regions = await regionsResponse.Content.ReadFromJsonAsync<List<RegionDto>>();
                Regions.Clear();
                foreach (var r in regions) Regions.Add(r);
            }

            if (commodityTypesResponse.IsSuccessStatusCode)
            {
                var commodityTypes = await commodityTypesResponse.Content.ReadFromJsonAsync<List<CommodityTypeDto>>();
                CommodityTypes.Clear();
                foreach (var ct in commodityTypes) CommodityTypes.Add(ct);
            }
        }

        private async Task LoadCommoditiesForSelectedType()
        {
            Commodities.Clear();
            
            if (SelectedCommodityType == null) return;

            var commoditiesResponse = await referenceDataApi.GetCommoditiesByIdAsync(SelectedCommodityType.Id);

            var commodities = await commoditiesResponse.Content.ReadFromJsonAsync<List<CommodityDto>>();
            Commodities.Clear();
            foreach (var c in commodities)
                Commodities.Add(c);
        }

        public bool CanPostBid => !HasErrors;

        [ObservableProperty] 
        private bool _showValidationErrors;

        [RelayCommand(CanExecute = nameof(CanPostBid))]
        public async Task PostBidAsync()
        {
            _showValidationErrors = true;
            ValidateAllProperties();

            if (HasErrors)
                return;

            var location = new LocationCommand
            {
                RegionId = SelectedRegion.Id,
                Town = Town,
                Quarter = Quarter,
                Street = Street
            };

            var userSession = sessionService.GetCurrentSessionAsync().Result;

            if (userSession != null && StartDate != null && EndDate != null)
            {           
                var command = new CreatePositionCommand
                    {
                        UserId = userSession.UserId,
                        CommodityId = SelectedCommodity.Id,
                        UnitPrice = UnitPrice,
                        Quantity = Quantity,
                        Grade = SelectedGrade,
                        Description = Description,
                        StartDate = StartDate.Value,
                        EndDate = EndDate.Value,
                        Origin = location
                    };

                await Shell.Current.DisplayAlert("Position Summary", $"Quantity: {command.Quantity} \n\n Unit Price: {command.UnitPrice} \n\n Duration: {command.EndDate - command.StartDate}","OK");
            }

            await Shell.Current.DisplayAlert("Error", "There was an error while placing your bid ...", "OK");
        }
    }
}