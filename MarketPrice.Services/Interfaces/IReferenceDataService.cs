using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketPrice.Domain.Reference;

namespace MarketPrice.Services.Interfaces
{
    public interface IReferenceDataService
    {
        Task<List<RegionDto>> GetRegionAsync();
        Task<List<CommodityTypeDto>> GetCommodityTypeAsync();
        Task<List<CommodityDto>> GetCommodityByIdAsync(Guid id);
    }
}
