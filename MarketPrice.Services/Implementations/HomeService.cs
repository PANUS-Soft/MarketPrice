using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPrice.Data;
using MarketPrice.Domain.Home.Dtos;
using MarketPrice.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MarketPrice.Services.Implementations
{
    public class HomeService(MarketPriceDbContext context) : IHomeService
    {
        private readonly MarketPriceDbContext _context = context;

        public async Task<List<LoadHomeResponseDto>> LoadHomeAsync()
        {
            // Constants
            const int BID_POSITION = 6001;   // Buy
            const int OFFER_POSITION = 6002; // Sell
            const int OPEN_STATUS = 5001;    // Open

            // 1. Load base commodity-type data with a JOIN to get the Image ID
            var commodityTypes = await (
                from ct in _context.CommodityTypes
                join c in _context.Commodities on ct.CommodityTypeId equals c.CommodityTypeId
                join uom in _context.UnitOfMeasures on c.UnitOfMeasureId equals uom.UnitOfMeasureId

                join cti in _context.CommodityTypeImage on ct.CommodityTypeId equals cti.CommodityTypeId into ctiGroup
                from cti in ctiGroup.DefaultIfEmpty() 

                select new
                {
                    ct.CommodityTypeId,
                    CommodityTypeName = ct.Name.LookupDataTextEnglish,

                    // FIX: Select the ID from the joined image table (handle null if no image exists)
                    CommodityTypeImageId = cti != null ? cti.CommodityTypeImageId : Guid.Empty,

                    ct.LastBestBid,
                    ct.LastBestOffer,
                    c.LotSize,
                    UnitOfMeasure = uom.UnitOfMeasureCodeEnglish
                }
            )
            .GroupBy(x => x.CommodityTypeId)
            .Select(g => g.First())
            .ToListAsync();

            var result = new List<LoadHomeResponseDto>();

            foreach (var item in commodityTypes)
            {
                // 2. Compute current best bid & offer from Positions
                var currentBestBid = await _context.Positions
                    .Where(p =>
                        p.Commodity.CommodityTypeId == item.CommodityTypeId &&
                        p.PositionTypeId == BID_POSITION &&
                        p.CurrentStatusId == OPEN_STATUS)
                    .MaxAsync(p => (decimal?)p.UnitPrice);

                var currentBestOffer = await _context.Positions
                    .Where(p =>
                        p.Commodity.CommodityTypeId == item.CommodityTypeId &&
                        p.PositionTypeId == OFFER_POSITION &&
                        p.CurrentStatusId == OPEN_STATUS)
                    .MinAsync(p => (decimal?)p.UnitPrice);

                currentBestBid ??= 0;
                currentBestOffer ??= 0;

                // 3. Determine trend
                bool isBidImproved = currentBestBid > item.LastBestBid;
                bool isOfferImproved = item.LastBestOffer == 0 || (currentBestOffer > 0 && currentBestOffer < item.LastBestOffer);

                var entity = await _context.CommodityTypes
                    .FirstAsync(ct => ct.CommodityTypeId == item.CommodityTypeId);

                if (entity.LastBestBid != currentBestBid || entity.LastBestOffer != currentBestOffer)
                {
                    entity.LastBestBid = (decimal)currentBestBid;
                    entity.LastBestOffer = (decimal)currentBestOffer;
                    entity.DateUpdated = DateTimeOffset.UtcNow;
                }

                // 5. Map to DTO
                result.Add(new LoadHomeResponseDto
                {
                    CommodityTypeId = item.CommodityTypeId,
                    CommodityTypeName = item.CommodityTypeName,
                    CommodityTypeImageId = item.CommodityTypeImageId,
                    ImageUrl = $"/api/commodity-types/{item.CommodityTypeId}/image",
                    LotSize = item.LotSize ?? 0,
                    UnitOfMeasure = item.UnitOfMeasure,
                    BestBidPrice = (decimal)currentBestBid,
                    BestOfferPrice = (decimal)currentBestOffer,
                    IsBidImproved = isBidImproved,
                    IsOfferImproved = isOfferImproved
                });
            }
            await _context.SaveChangesAsync();
            return result;
        }
    }
}