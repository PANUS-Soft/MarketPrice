namespace MarketPrice.Data.Models
{
    public class DeliveryDetail
    {
        public Guid DeliveryDetailId { get; set; }
        public Guid PositionId { get; set; }
        public Guid OriginLocationId { get; set; }
        public Location OriginLocation { get; set; } = null;
        public Guid? DestinationLocationId { get; set; }
        public Location? DestinationLocation { get; set; }
        public required bool IsDeliverable { get; set; }
        public string? LeadTimeInDays { get; set; }
        public decimal? Fee { get; set; }
        public decimal? MaxDistance { get; set; }
    }
}
