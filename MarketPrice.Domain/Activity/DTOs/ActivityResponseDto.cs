using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Activity.DTOs
{
    public class ActivityResponseDto
    {
        public string CommodityName { get; set; }
        public decimal Quantity { get; set; }
        public string PositionType { get; set; }
        public string State { get; set; }
        public decimal Price { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
