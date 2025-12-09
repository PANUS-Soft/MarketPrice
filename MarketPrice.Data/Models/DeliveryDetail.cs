using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Data.Models
{
    public class DeliveryDetail
    {
        public Guid DeliveryDetailId { get; set; }
        public Guid PositionId { get; set; }
        public Guid LocationId { get; set; }
        public required bool IsDeliverable { get; set; }
        public required string LeadTime { get; set; }
        public decimal? Fee { get; set; }
        public required decimal MaxDistance { get; set; }
    }
}
