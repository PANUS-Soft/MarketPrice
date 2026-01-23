using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Market.Commands
{
    public class MarketInsightCommand
    {
        public Guid CommodityTypeId { get; set; } = Guid.Empty;
    }
}
