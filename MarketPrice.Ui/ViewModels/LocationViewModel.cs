using CommunityToolkit.Mvvm.ComponentModel;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Domain.Reference;
using System.ComponentModel.DataAnnotations;

namespace MarketPrice.Ui.ViewModels
{
    public partial class LocationViewModel : ObservableValidator
    {
        [ObservableProperty] [Required(ErrorMessage = "Region is required.")]
        private RegionDto _selectedRegion;

        [ObservableProperty] [Required(ErrorMessage = "Town is required.")]
        private string _town;

        [ObservableProperty] [Required(ErrorMessage = "Quarter is required.")]
        private string _quarter;

        [ObservableProperty] [Required(ErrorMessage = "Street is required.")]
        private string _street;

        public bool Validate()
        {
            ValidateAllProperties();
            return !HasErrors;
        }

        public LocationCommand ToCommand()
        {
            return new LocationCommand
            {
                RegionId = SelectedRegion.Id,
                Town = Town,
                Quarter = Quarter,
                Street = Street
            };
        }
    }
}
