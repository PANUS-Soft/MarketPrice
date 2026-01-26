using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Data.Models
{
    public class CommodityTypeImage
    {
        public Guid CommodityTypeImageId { get; set; }
        public Guid CommodityTypeId { get; set; }
        public required byte[] ImageData { get; set; }
        public required string ContentType { get; set; }
        public required string FileName { get; set; }

        //the commodity table navigate to commodityType Imange!
        public required CommodityType CommodityType { get; set; }
    }
}
