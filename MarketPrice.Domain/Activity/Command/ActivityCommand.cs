using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Activity.Command
{
    public class ActivityCommand
    {
        public string? PositionType { get; set; }
        public Guid? CommodityId { get; set; }

    }
}
