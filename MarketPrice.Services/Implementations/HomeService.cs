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
            const int BID_POSITION = 6001;
            const int OFFER_POSITION = 6002;

            // 1. Single Query: Join tables explicitly and calculate prices in the database
            var marketData = await (
                from ct in _context.CommodityTypes
                join c in _context.Commodities on ct.CommodityTypeId equals c.CommodityTypeId
                join uom in _context.UnitOfMeasures on c.UnitOfMeasureId equals uom.UnitOfMeasureId
                join cti in _context.CommodityTypeImage on ct.CommodityTypeId equals cti.CommodityTypeId into ctiGroup
                from cti in ctiGroup.DefaultIfEmpty()
                select new
                {
                    Entity = ct,
                    TypeName = ct.Name.LookupDataTextEnglish,
                    LotSize = c.LotSize,
                    UomCode = uom.UnitOfMeasureCodeEnglish,
                    ImageId = cti != null ? cti.CommodityTypeImageId : Guid.Empty,

                    CurrentBid = _context.Positions
                        .Where(p => p.Commodity.CommodityTypeId == ct.CommodityTypeId &&
                                    p.PositionTypeId == BID_POSITION &&
                                    p.StartDate <= DateTime.UtcNow && p.ExpiryDate > DateTime.UtcNow)
                        .Max(p => (decimal?)p.UnitPrice) ?? 0m,

                    CurrentOffer = _context.Positions
                        .Where(p => p.Commodity.CommodityTypeId == ct.CommodityTypeId &&
                                    p.PositionTypeId == OFFER_POSITION &&
                                    p.StartDate <= DateTime.UtcNow && p.ExpiryDate > DateTime.UtcNow)
                        .Min(p => (decimal?)p.UnitPrice) ?? 0m
                }
            )
            .GroupBy(x => x.Entity.CommodityTypeId)
            .Select(g => g.First())
            .ToListAsync();

            var result = new List<LoadHomeResponseDto>();

            foreach (var x in marketData)
            {
                var item = x.Entity;

                // --- 2. STICKY TREND LOGIC ---
                // Only update the boolean if the price has actually moved.

                // BID LOGIC: Higher is better
                if (x.CurrentBid > 0)
                {
                    if (x.CurrentBid > item.LastBestBid)
                        item.IsBidImproved = true;
                    else if (x.CurrentBid < item.LastBestBid)
                        item.IsBidImproved = false;

                    item.LastBestBid = x.CurrentBid;
                }

                // OFFER LOGIC: Lower is better
                if (x.CurrentOffer > 0)
                {
                    if (item.LastBestOffer == 0 || x.CurrentOffer < item.LastBestOffer)
                        item.IsOfferImproved = true;
                    else if (x.CurrentOffer > item.LastBestOffer)
                        item.IsOfferImproved = false;

                        item.LastBestOffer = x.CurrentOffer;
                }

                item.DateUpdated = DateTimeOffset.UtcNow;
                // In case GroupBy broke tracking, this ensures the update is sent.
                _context.Entry(item).State = EntityState.Modified;

                result.Add(new LoadHomeResponseDto
                {
                    CommodityTypeId = item.CommodityTypeId,
                    CommodityTypeName = x.TypeName,
                    CommodityTypeImageId = x.ImageId,
                    ImageUrl = $"{ApiControllers.CommodityTypeImages}/{item.CommodityTypeId}/image",
                    LotSize = x.LotSize ?? 0,
                    UnitOfMeasure = x.UomCode,
                    BestBidPrice = x.CurrentBid,
                    BestOfferPrice = x.CurrentOffer,
                    IsBidImproved = item.IsBidImproved,
                    IsOfferImproved = item.IsOfferImproved
                });
            }

            // 3. Save all state changes in one transaction
            await _context.SaveChangesAsync();

            return result;
        }
    }
}