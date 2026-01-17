using System.ComponentModel.DataAnnotations;
using MarketPrice.Domain.Reference;

namespace MarketPrice.Ui.Models
{
    public class LocationInformation
    {
        [Required(ErrorMessage = "Region is required")]
        public required RegionDto Region { get; set; }

        [Required(ErrorMessage = "Street name is required")]
        public required string Street { get; set; }

        [Required(ErrorMessage = "Town is required")]
        public required string Town { get; set; }

        [Required(ErrorMessage = "Quarter is required")]
        public required string Quarter { get; set; }
    }
}
