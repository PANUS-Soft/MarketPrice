using MarketPrice.Data;
using MarketPrice.Domain;
using MarketPrice.Domain.Home.DTOs;
using MarketPrice.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using MarketPrice.Data.Models;

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

                    join ci in _context.CommodityImage on c.CommodityId equals ci.CommodityId into ciGroup
                    from ci in ciGroup.DefaultIfEmpty()

                    where p.StartDate <= now && p.ExpiryDate > now
                          && ld.LookupDataTypeId == 7000

                    select new
                    {
                        ct.CommodityTypeId,
                        TypeName = ct.Name.LookupDataTextEnglish,
                        c.CommodityId,
                        c.CommodityName,
                        CommodityEntity = c, // Tracking trend flags on the Commodity level now
                        CommodityImageId = ci != null ? ci.CommodityImageId : Guid.Empty,
                        c.LotSize,
                        UnitOfMeasureCode = uom.UnitOfMeasureCodeEnglish,
                        Position = p,
                        LocationName = ld.LookupDataTextEnglish
                    }
                ).ToListAsync();

            var result = new List<LoadHomeResponseDto>();

            // Group 1: By Commodity Type (e.g., CORN)
            var typeGroups = flatData.GroupBy(x => x.CommodityTypeId);

            foreach (var typeGroup in typeGroups)
            {
                var typeDto = new LoadHomeResponseDto
                {
                    CommodityTypeId = typeGroup.Key,
                    CommodityTypeName = typeGroup.First().TypeName
                };

                // Group 2: By specific Commodity within that Type (e.g., Fresh Corn vs Dry Corn)
                var commodityGroups = typeGroup.GroupBy(x => x.CommodityId);

                foreach (var commGroup in commodityGroups)
                {
                    var firstComm = commGroup.First();
                    var commodity = firstComm.CommodityEntity;

                    var bids = commGroup.Where(x => x.Position.PositionTypeId == BidPosition).ToList();
                    var offers = commGroup.Where(x => x.Position.PositionTypeId == AskPosition).ToList();

                    var bestBidPos = bids.OrderByDescending(x => x.Position.UnitPrice).FirstOrDefault();
                    var bestOfferPos = offers.OrderBy(x => x.Position.UnitPrice).FirstOrDefault();

                    // Trend Logic for the specific Commodity
                    _context.Attach(commodity);
                    UpdateCommodityTrends(commodity, bestBidPos?.Position.UnitPrice ?? 0, bestOfferPos?.Position.UnitPrice ?? 0);

                    var commDetail = new HomeCommodityDetailDto
                    {
                        CommodityId = firstComm.CommodityId,
                        CommodityName = firstComm.CommodityName,
                        CommodityImageId = firstComm.CommodityImageId,
                        ImageUrl = $"{ApiControllers.CommodityImages}/{firstComm.CommodityId}/image",
                        LotSize = (short?)firstComm.LotSize,
                        UnitOfMeasure = firstComm.UnitOfMeasureCode,

                        IsBidImproved = commodity.IsBidImproved,
                        IsOfferImproved = commodity.IsOfferImproved,
                        IsBidSoonToExpire = bestBidPos != null && IsExpired(bestBidPos.Position.StartDate, bestBidPos.Position.ExpiryDate, now),
                        IsOfferSoonToExpire = bestOfferPos != null && IsExpired(bestOfferPos.Position.StartDate, bestOfferPos.Position.ExpiryDate, now),

                        BidDepth = bids.GroupBy(b => b.Position.UnitPrice)
                                       .OrderByDescending(g => g.Key)
                                       .Select(g => new HomeMarketDepthDto
                                       {
                                           Price = g.Key,
                                           Locations = g.Select(x => x.LocationName).Distinct().ToList(),
                                           TotalActivePosforPrice = g.Count()
                                       }).ToList(),

                        OfferDepth = offers.GroupBy(o => o.Position.UnitPrice)
                                         .OrderBy(g => g.Key)
                                         .Select(g => new HomeMarketDepthDto
                                         {
                                             Price = g.Key,
                                             Locations = g.Select(x => x.LocationName).Distinct().ToList(),
                                             TotalActivePosforPrice = g.Count()
                                         }).ToList()
                    };

                    typeDto.Commodities.Add(commDetail);
                }
                result.Add(typeDto);
            }

            await _context.SaveChangesAsync();
            return result;
        }

        // Helper method used to calculate soon-to-expire for positions
        private bool IsExpired(DateTime start, DateTime expiry, DateTime now)
        {
            var total = (expiry - start).TotalSeconds;
            if (total <= 0) return false;
            return ((now - start).TotalSeconds / total) >= 0.8;
        }

        // Private helper to keep the loop clean
        private void UpdateCommodityTrends(Commodity commodity, decimal currentBestBid, decimal currentBestOffer)
        {
            // 2. STICKY BID TREND LOGIC
            // Only update the flag if the price has actually moved.
            if (currentBestBid > 0 && commodity.LastBestBid > 0)
            {
                if (currentBestBid > commodity.LastBestBid)
                {
                    commodity.IsBidImproved = true; // Upward trend
                }
                else if (currentBestBid < commodity.LastBestBid)
                {
                    commodity.IsBidImproved = false; // Downward trend
                }
                else
                {
                    commodity.IsBidImproved = commodity.IsBidImproved;
                }
            }

            // 3. STICKY OFFER TREND LOGIC
            if (currentBestOffer > 0 && commodity.LastBestOffer > 0)
            {
                if (currentBestOffer < commodity.LastBestOffer)
                {
                    commodity.IsOfferImproved = true; // Price improved (dropped)
                }
                else if (currentBestOffer > commodity.LastBestOffer)
                {
                    commodity.IsOfferImproved = false; // Price declined (rose)
                }
                else
                {
                    commodity.IsOfferImproved = commodity.IsOfferImproved;
                }
            }
            commodity.LastBestBid = currentBestBid;
            commodity.LastBestOffer = currentBestOffer;
            commodity.DateUpdated = DateTime.UtcNow;
            _context.Entry(commodity).State = EntityState.Modified;
        }
    }
}
