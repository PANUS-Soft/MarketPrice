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

            // 1. Get CommodityTypes and their child Commodities + Active Positions in one query
            var data = await _context.CommodityTypes
                .AsNoTracking()
                .Select(ct => new
                {
                    TypeEntity = ct,
                    TypeName = ct.Name.LookupDataTextEnglish,
                    ImageId = _context.CommodityTypeImage
                        .Where(cti => cti.CommodityTypeId == ct.CommodityTypeId)
                        .Select(cti => (Guid?)cti.CommodityTypeImageId)
                        .FirstOrDefault() ?? Guid.Empty,

                    ActivePrices = _context.Commodities
                        .Where(c => c.CommodityTypeId == ct.CommodityTypeId)
                        .Select(c => new
                        {
                            c.LotSize,
                            UomCode = c.UnitOfMeasure.UnitOfMeasureCodeEnglish,
                            BestBid = _context.Positions
                                .Where(p => p.CommodityId == c.CommodityId &&
                                            p.PositionTypeId == BidPosition &&
                                            p.StartDate <= now && p.ExpiryDate > now)
                                .Max(p => (decimal?)p.UnitPrice) ?? 0,
                            BestOffer = _context.Positions
                                .Where(p => p.CommodityId == c.CommodityId &&
                                            p.PositionTypeId == AskPosition &&
                                            p.StartDate <= now && p.ExpiryDate > now)
                                .Min(p => (decimal?)p.UnitPrice) ?? 0
                        }).ToList()
                }).ToListAsync();

            var result = new List<LoadHomeResponseDto>();

            foreach (var x in data)
            {
                var ct = x.TypeEntity;

                // Determine the Live Best Bid/Offer across all commodities of this type
                var currentBestBid = x.ActivePrices.Any() ? x.ActivePrices.Max(p => p.BestBid) : 0;
                var currentBestOffer = x.ActivePrices.Any(p => p.BestOffer > 0)
                    ? x.ActivePrices.Where(p => p.BestOffer > 0).Min(p => p.BestOffer)
                    : 0;

                _context.Attach(ct);

                // 2. STICKY BID TREND LOGIC
                // Only update the flag if the price has actually moved.
                if (currentBestBid > 0 && ct.LastBestBid > 0)
                {
                    if (currentBestBid > ct.LastBestBid)
                    {
                        ct.IsBidImproved = true; // Upward trend
                    }
                    else if (currentBestBid < ct.LastBestBid)
                    {
                        ct.IsBidImproved = false; // Downward trend
                    }
                    else
                    {
                        ct.IsBidImproved = ct.IsBidImproved;
                    }
                }

                // 3. STICKY OFFER TREND LOGIC
                if (currentBestOffer > 0 && ct.LastBestOffer > 0)
                {
                    if (currentBestOffer < ct.LastBestOffer)
                    {
                        ct.IsOfferImproved = true; // Price improved (dropped)
                    }
                    else if (currentBestOffer > ct.LastBestOffer)
                    {
                        ct.IsOfferImproved = false; // Price declined (rose)
                    }
                    else
                    {
                        ct.IsOfferImproved = ct.IsOfferImproved;
                    }
                }

                // 4. Update Reference Prices
                // Note: If market is 0, we update LastBest to 0 to stop "stale" prices from showing,
                // but the trend flags above were skipped, so they stay in their last state.
                ct.LastBestBid = currentBestBid;
                ct.LastBestOffer = currentBestOffer;
                ct.DateUpdated = DateTime.UtcNow;

                var firstItem = x.ActivePrices.FirstOrDefault();

                result.Add(new LoadHomeResponseDto
                {
                    CommodityTypeId = ct.CommodityTypeId,
                    CommodityTypeName = x.TypeName,
                    CommodityTypeImageId = x.ImageId,
                    ImageUrl = $"{ApiControllers.CommodityTypeImages}/{ct.CommodityTypeId}/image",
                    LotSize = firstItem?.LotSize ?? 0,
                    UnitOfMeasure = firstItem?.UomCode ?? "N/A",
                    BestBidPrice = currentBestBid,
                    BestOfferPrice = currentBestOffer,
                    IsBidImproved = ct.IsBidImproved,
                    IsOfferImproved = ct.IsOfferImproved
                });
            }

            await _context.SaveChangesAsync();
            return result;
        }
    }
}
