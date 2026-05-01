using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Ui.Common;

namespace MarketPrice.Ui.Models
{
    public class Activity
    {
        public Guid PositionId { get; set; }
        public Guid CommodityTypeId { get; set; }
        public Guid CommodityId { get; set; }
        public string CommodityName { get; set; }
        public string Quantity { get; set; }
        public string Grade { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string State { get; set; }
        public Color StateColor { get; set; }
        public string ImageUrl { get; set; }
        public DateTimeOffset Date { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public PositionType PositionType { get; set; }
        public string PosType { get; set; } // <-- Added this (e.g., "Bid" or "Offer")
        public string UnitOfMeasure { get; set; }
        public string LotSize { get; set; }
        public bool IsDeliverable { get; set; }
        public string LeadTime { get; set; }
        public string DeliveryFee { get; set; }
        public int ShelfLifeInDays { get; set; }
        public string OriginRegion { get; set; }
        public string DestinationRegion { get; set; }
        public LocationCommand? Origin { get; set; }
        public LocationCommand? Destination { get; set; }
        public string TotalQuantity { get; set; }
        public string TotalPrice { get; set; }
    }

    public class ActivityGroup : List<Activity>
    {
        public string Name { get; private set; }
        public ActivityGroup(string name, List<Activity> items) : base(items) { Name = name; }
    }
}
