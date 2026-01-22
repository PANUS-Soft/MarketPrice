using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Position.Commands
{
    public class PositionCommand
    {
        public Guid UserId { get; set; }
        public Guid CommodityId { get; set; }
        public required decimal UnitPrice { get; set; }
        public required decimal Quantity { get; set; }
        public required string Grade { get; set; }
        public string? Description { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public string? LeadTime { get; set; }
        public decimal? DeliveryFee { get; set; }
        public decimal? MaxDistance { get; set; }
        public required LocationCommand Origin { get; set; }
        public LocationCommand? Destination
        {
            get; set;
        }
    }
}
