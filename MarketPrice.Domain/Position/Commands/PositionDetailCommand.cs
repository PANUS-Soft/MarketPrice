using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Position.Commands
{
    public class PositionDetailCommand
    {
        public Guid UserId { get; set; }
        public Guid PositionId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string  CommodityName { get; set; } = string.Empty ;
    }
}
