namespace MarketPrice.Domain.Reference
{
    public class CommodityTypeDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? UnitOfMeasure { get; set; } = string.Empty;
    }
}
