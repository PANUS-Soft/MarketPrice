using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Position.Commands
{
    public class PositionListingCommand
    {
        public Guid CommodityTypeId { get; set; }
        public Guid? CommodityId { get; set; }
        public int PositionTypeId { get; set; }
        public decimal UnitPrice { get; set; }
        public string? CommodityName { get; set; }
    }
}
