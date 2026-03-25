using LinqToDB.Async;
using MarketPrice.Data;
using MarketPrice.Domain.Reference.DTOs;
using MarketPrice.Services.Interfaces;

namespace MarketPrice.Services.Implementations
{
    public class ReferenceDataService(MarketPriceDbContext context) : IReferenceDataService
    {
        public async Task<List<RegionDto>> GetRegionAsync()
        {
            return await context.LookupData.Where(ld => ld.LookupDataTypeId == 7000).Select(x => new RegionDto
            {
                Id = x.LookupDataId,
                NameInEnglish = x.LookupDataTextEnglish
            }).ToListAsync();

        }

        public async Task<List<CommodityTypeDto>> GetCommodityTypeAsync()
        {
            return await context.CommodityTypes.Select(ct => new CommodityTypeDto
            {
                Id = ct.CommodityTypeId,
                Code = ct.Code,
                Name = context.LookupData.Where(ld => ld.LookupDataId == ct.NameId).Select(ld => ld.LookupDataTextEnglish).FirstOrDefault(),
                UnitOfMeasure = context.UnitOfMeasures.Where(um => um.UnitOfMeasureId == ct.DefaultUnitOfMeasureId).Select(um => um.UnitOfMeasureCodeEnglish).FirstOrDefault()
            }).ToListAsync();
        }

        public async Task<List<CommodityDto>> GetCommodityByIdAsync(Guid id)
        {
            return await context.Commodities.Where(c => c.CommodityTypeId == id).Select(c => new CommodityDto
            {
                Id = c.CommodityId,
                CommodityTypeId = c.CommodityTypeId,
                ShelfLifeInDays = c.ShelfLifeInDays,
                LotSize = c.LotSize,
                Name = c.CommodityName
            }).ToListAsync();
        }

        public async Task<List<CommodityDto>> GetAllCommoditiesAsync()
        {
            return await context.Commodities.Select(c => new CommodityDto
            {
                Id = c.CommodityId,
                CommodityTypeId = c.CommodityTypeId,
                Name = c.CommodityName
            }).ToListAsync();
        }
    }
}
