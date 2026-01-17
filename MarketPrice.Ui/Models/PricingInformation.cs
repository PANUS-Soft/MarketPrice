using System.ComponentModel.DataAnnotations;

namespace MarketPrice.Ui.Models
{
    public class PricingInformation
    {
        [Range(1.00, double.MaxValue, ErrorMessage = "Unit Price must be positive")]
        public decimal UnitPrice { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        [Required]
        public TimeOnly EndTime { get; set; }
        
        public bool IsDeliverable { get; set; }
    }
}
