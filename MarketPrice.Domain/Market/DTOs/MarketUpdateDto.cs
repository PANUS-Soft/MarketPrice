using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Domain.Market.DTOs
{
    public class MarketUpdateDto
    {
        public Guid CommodityId { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; }
        public decimal Quantity { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
