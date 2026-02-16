using MarketPrice.Data;
using MarketPrice.Domain;
using MarketPrice.Domain.Home.DTOs;
using MarketPrice.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MarketPrice.Services.Implementations
{
    public class HomeService(MarketPriceDbContext context) : IHomeService
    {
        private readonly MarketPriceDbContext _context = context;

        public async Task<List<LoadHomeResponseDto>> LoadHomeAsync()
        {
            // Load CommodityTypes + metadata
            var commodityTypes = await (
                from ct in _context.CommodityTypes
                join cti in _context.CommodityTypeImage
                    on ct.CommodityTypeId equals cti.CommodityTypeId into ctiGroup
                from cti in ctiGroup.DefaultIfEmpty()
                select new
                {
                    Entity = ct,
                    TypeName = ct.Name.LookupDataTextEnglish,
                    ImageId = cti != null ? cti.CommodityTypeImageId : Guid.Empty
                }
            ).ToListAsync();

            var result = new List<LoadHomeResponseDto>();

            foreach (var x in commodityTypes)
            {
                var ct = x.Entity;

                // Aggregate prices from COMMODITIES (single source of truth)
                var commodityPrices = await _context.Commodities
                    .Where(c => c.CommodityTypeId == ct.CommodityTypeId)
                    .Select(c => new
                    {
                        c.LastBestBid,
                        c.LastBestOffer,
                        c.LotSize,
                        UnitOfMeasure = c.UnitOfMeasure.UnitOfMeasureCodeEnglish
                    })
                    .ToListAsync();

                if (!commodityPrices.Any())
                    continue;

                var currentBestBid = commodityPrices.Max(x => x.LastBestBid);

                var currentBestOffer = commodityPrices
                    .Where(x => x.LastBestOffer > 0)
                    .Select(x => x.LastBestOffer)
                    .DefaultIfEmpty(0)
                    .Min();

                // Sticky trend logic (CommodityType)
                if (currentBestBid > ct.LastBestBid)
                    ct.IsBidImproved = true;
                else if (currentBestBid < ct.LastBestBid)
                    ct.IsBidImproved = false;

                if (ct.LastBestOffer == 0 || currentBestOffer < ct.LastBestOffer)
                    ct.IsOfferImproved = true;
                else if (currentBestOffer > ct.LastBestOffer)
                    ct.IsOfferImproved = false;

                ct.LastBestBid = currentBestBid;
                ct.LastBestOffer = currentBestOffer;
                ct.DateUpdated = DateTime.UtcNow;

                _context.Entry(ct).State = EntityState.Modified;

                // Build response
                var firstCommodity = commodityPrices.First();

                result.Add(new LoadHomeResponseDto
                {
                    CommodityTypeId = ct.CommodityTypeId,
                    CommodityTypeName = x.TypeName,
                    CommodityTypeImageId = x.ImageId,
                    ImageUrl = $"CommodityTypeImages/{ct.CommodityTypeId}/image",
                    LotSize = firstCommodity.LotSize ?? 0,
                    UnitOfMeasure = firstCommodity.UnitOfMeasure,
                    BestBidPrice = currentBestBid,
                    BestOfferPrice = currentBestOffer,
                    IsBidImproved = ct.IsBidImproved,
                    IsOfferImproved = ct.IsOfferImproved
                });
            }

            // Persist all CommodityType state changes
            await _context.SaveChangesAsync();

            return result;
        }
    }
}
