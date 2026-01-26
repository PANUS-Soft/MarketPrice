using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Data.Models
{
    public class CommodityImage
    {
        public Guid CommodityImageId { get; set; }
        public Guid CommodityId { get; set; }
        public required byte[] ImageData { get; set; }
        public required string ContentType { get; set; }
        public required string FileName { get; set; }
        // Navigation property to Commodity table
        public required Commodity Commodity { get; set; }

    }
}
