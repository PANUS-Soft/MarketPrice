using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Data.Models
{
    public  class CommodityImage
    {
        public Guid CommodityImageId { get; set; }
        public required byte[] ImageData { get; set; }
        public required string ContentType {  get; set; }
        public required string FileName { get; set; }

    }
}
