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

        private const int BidPosition = 6001;  // Bid
        private const int AskPosition = 6002;  // Offer

        public async Task<List<LoadHomeResponseDto>> LoadHomeAsync()
        {
            var now = DateTime.UtcNow;

            var flatData = await (
                    from ct in _context.CommodityTypes
                    join c in _context.Commodities on ct.CommodityTypeId equals c.CommodityTypeId
                    join p in _context.Positions.AsNoTracking() on c.CommodityId equals p.CommodityId
                    join dd in _context.DeliveryDetails on p.PositionId equals dd.PositionId
                    join loc in _context.Locations on dd.OriginLocationId equals loc.LocationId
                    join ld in _context.LookupData on loc.RegionId equals ld.LookupDataId
                    join uom in _context.UnitOfMeasures on c.UnitOfMeasureId equals uom.UnitOfMeasureId

                    // Join for the Category/Type Image
                    join cti in _context.CommodityTypeImage on ct.CommodityTypeId equals cti.CommodityTypeId into ctiGroup
                    from cti in ctiGroup.DefaultIfEmpty()

                    where p.StartDate <= now && p.ExpiryDate > now
                          && ld.LookupDataTypeId == 7000

                    select new
                    {
                        ct.CommodityTypeId,
                        CommodityTypeEntity = ct, // Keep entity for tracking updates
                        TypeName = ct.Name.LookupDataTextEnglish,
                        CommodityTypeImageId = cti != null ? cti.CommodityTypeImageId : Guid.Empty,

                        // Commodity & Position Info
                        c.LotSize,
                        UnitOfMeasureCode = uom.UnitOfMeasureCodeEnglish,
                        Position = p,
                        LocationName = ld.LookupDataTextEnglish
                    }
                ).ToListAsync();

            // 2. Group by CommodityTypeId to aggregate data for the Type (e.g., "BEANS", "CORN")
            var groupedData = flatData
                .GroupBy(x => x.CommodityTypeId)
                .ToList();

            var result = new List<LoadHomeResponseDto>();

            foreach (var group in groupedData)
            {
                var first = group.First();
                var ctEntity = first.CommodityTypeEntity;

                // Flatten all positions under this CommodityType
                var allPositions = group.Select(g => new { g.Position, g.LocationName }).ToList();
                var bids = allPositions.Where(ap => ap.Position.PositionTypeId == BidPosition).ToList();
                var offers = allPositions.Where(ap => ap.Position.PositionTypeId == AskPosition).ToList();

                // 3. Identify Best Positions across the whole Type
                var bestBidPos = bids.OrderByDescending(ap => ap.Position.UnitPrice).FirstOrDefault();
                var bestOfferPos = offers.OrderBy(ap => ap.Position.UnitPrice).FirstOrDefault();

                var currentBestBid = bestBidPos?.Position.UnitPrice ?? 0m;
                var currentBestOffer = bestOfferPos?.Position.UnitPrice ?? 0m;

                // 4. Trend / Improvement Logic (Sticky Logic)
                _context.Attach(ctEntity);
            
                // 2. STICKY BID TREND LOGIC
                // Only update the flag if the price has actually moved.
                if (currentBestBid > 0 && ctEntity.LastBestBid > 0)
                {
                    if (currentBestBid > ctEntity.LastBestBid)
                    {
                        ctEntity.IsBidImproved = true; // Upward trend
                    }
                    else if (currentBestBid < ctEntity.LastBestBid)
                    {
                        ctEntity.IsBidImproved = false; // Downward trend
                    }
                    else
                    {
                        ctEntity.IsBidImproved = ctEntity.IsBidImproved;
                    }
                }

                // 3. STICKY OFFER TREND LOGIC
                if (currentBestOffer > 0 && ctEntity.LastBestOffer > 0)
                {
                    if (currentBestOffer < ctEntity.LastBestOffer)
                    {
                        ctEntity.IsOfferImproved = true; // Price improved (dropped)
                    }
                    else if (currentBestOffer > ctEntity.LastBestOffer)
                    {
                        ctEntity.IsOfferImproved = false; // Price declined (rose)
                    }
                    else
                    {
                        ctEntity.IsOfferImproved = ctEntity.IsOfferImproved;
                    }
                }

                ctEntity.DateUpdated = DateTime.UtcNow;
                _context.Entry(ctEntity).State = EntityState.Modified;

                // 5. Build the DTO
                result.Add(new LoadHomeResponseDto
                {
                    CommodityTypeId = first.CommodityTypeId,
                    CommodityTypeName = first.TypeName,
                    CommodityTypeImageId = first.CommodityTypeImageId,
                    ImageUrl = $"CommodityTypeImages/{first.CommodityTypeId}/image",
                    LotSize = first.LotSize,
                    UnitOfMeasure = first.UnitOfMeasureCode,

                    BestBidPrice = currentBestBid,
                    BestOfferPrice = currentBestOffer,

                    IsBidImproved = ctEntity.IsBidImproved,
                    IsOfferImproved = ctEntity.IsOfferImproved,

                    // Expiry logic using your IsExpired helper
                    IsBidSoonToExpire = bestBidPos != null && IsExpired(bestBidPos.Position.StartDate, bestBidPos.Position.ExpiryDate, now),
                    IsOfferSoonToExpire = bestOfferPos != null && IsExpired(bestOfferPos.Position.StartDate, bestOfferPos.Position.ExpiryDate, now)
                });
            }

            await _context.SaveChangesAsync();
            return result;
        }
    }
}
