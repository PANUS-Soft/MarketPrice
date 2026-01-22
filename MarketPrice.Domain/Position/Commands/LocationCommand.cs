namespace MarketPrice.Domain.Position.Commands
{
    public class LocationCommand
    {
        public int RegionId { get; set; }
        public required string Town { get; set; }
        public required string Quarter { get; set; }
        public string? Street { get; set; }
    }
}
