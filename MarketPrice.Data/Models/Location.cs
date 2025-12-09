using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Data.Models
{
    public class Location
    {
        public Guid LocationId { get; set; }
        public int LocationTypeId { get; set; }
        public Guid UserId { get; set; }
        public required int RegionId { get; set; }
        public string? Street { get; set; }
        public required string Town { get; set; }
        public required string Quarter { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
    }   
}
