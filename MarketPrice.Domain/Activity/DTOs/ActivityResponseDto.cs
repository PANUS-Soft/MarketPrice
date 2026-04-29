using MarketPrice.Domain.Position.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Activity.DTOs
{
    public class ActivityResponseDto
    {
        public Guid CommodityId { get; set; }
        public Guid CommodityTypeId { get; set; }
        public string CommodityName { get; set; }
        public decimal Quantity { get; set; }
        public string Grade { get; set; }
        public string? Description { get; set; }
        public string PositionType { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string State { get; set; }
        public decimal UnitPrice { get; set; }
        public bool CanDeliver { get; set; }
        public string? LeadTime { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public decimal? DeliveryFee { get; set; }
        public string UnitOfMeasure { get; set; }
        public short? LotSize { get; set; }
        public int ShelfLifeInDays { get; set; }
        public LocationCommand? Origin { get; set; }
        public LocationCommand? Destination { get; set; }
        public string? OriginRegion { get; set; }
        public string? DestinationRegion { get; set; }
    }
}
