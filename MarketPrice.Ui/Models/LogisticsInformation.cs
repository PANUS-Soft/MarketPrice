using System.ComponentModel.DataAnnotations;

namespace MarketPrice.Ui.Models
{
    public class LogisticsInformation
    {
        public LocationInformation OriginLocation { get; set; }
        public LocationInformation? DestinationLocation { get; set; }
        public string? LeadTime { get; set; }

        [Range(0, 10000, ErrorMessage = "Distance must be a positive value")]
        public decimal MaxDistance { get; set; }

        [Range(0.00, double.MaxValue, ErrorMessage = "Delivery fee must be a positive value")]
        [DataType(DataType.Currency)]
        public decimal DeliveryFee { get; set; }
    }
}
