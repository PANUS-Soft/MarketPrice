using System.ComponentModel.DataAnnotations;
using MarketPrice.Domain.Reference;

namespace MarketPrice.Ui.Models
{
    public class CommodityDetails
    {
        [Required(ErrorMessage = "Please select a commodity")]
        public CommodityDto Commodity { get; set; }
        public string Grade { get; set; }

        [Range(1.00, double.MaxValue, ErrorMessage = "Quantity must be at least 1.00")]
        public decimal Quantity { get; set; }
        public string? Description { get; set; }
    }
}
