using MarketPrice.Domain.Home.DTOs;
using MarketPrice.Ui.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Ui.Mappers
{
    public static class HomeMapper
    {
        public static ObservableCollection<CommodityGroupDisplayModel> ToDisplayGroups(List<LoadHomeResponseDto> response)
        {
            var groups = new ObservableCollection<CommodityGroupDisplayModel>();

            foreach (var group in response)
            {
                var displayGroup = new CommodityGroupDisplayModel()
                {
                    CommodityTypeId = group.CommodityTypeId,
                    GroupName = group.CommodityTypeName?.ToUpperInvariant() ?? string.Empty,
                };

                foreach (var dto in group.Commodities)
                {
                    displayGroup.Commodities.Add(ToDisplayModel(dto));
                }

                groups.Add(displayGroup);
            }
            return groups;
        }

        private static CommodityDisplayModel ToDisplayModel(HomeCommodityDetailDto dto)
        {
            var bid0 = dto.BidDepth.ElementAtOrDefault(0);
            var bid1 = dto.BidDepth.ElementAtOrDefault(1);
            var bid2 = dto.BidDepth.ElementAtOrDefault(2);

            var offer0 = dto.OfferDepth.ElementAtOrDefault(0);
            var offer1 = dto.OfferDepth.ElementAtOrDefault(1);
            var offer2 = dto.OfferDepth.ElementAtOrDefault(2);  

            return new CommodityDisplayModel()
            {
                CommodityId = dto.CommodityId,
                Name = dto.CommodityName ?? string.Empty,
                ImageUrl = dto.ImageUrl ?? string.Empty,
                LotSizeDisplay = dto.LotSize.HasValue ? $"{dto.LotSize} {dto.UnitOfMeasure}" : dto.UnitOfMeasure ?? string.Empty,

                IsBidImproved = dto.IsBidImproved,
                IsOfferImproved = dto.IsOfferImproved,
                IsBidSoonToExpire = dto.IsBidSoonToExpire,
                IsOfferSoonToExpire = dto.IsOfferSoonToExpire,

                BestBidPrice = bid0?.Price ?? 0,
                BestBidQuantity = bid0?.TotalActivePosforPrice ?? 0,
                BestBidLocation = bid0?.Locations.FirstOrDefault() ?? string.Empty,

                NextBid1 = bid1?.Price ?? 0,
                NextBid2 = bid2?.Price ?? 0,

                BestOfferPrice = offer0?.Price ?? 0,
                BestOfferQuantity = offer0?.TotalActivePosforPrice ?? 0,
                BestOfferLocation = offer0?.Locations.FirstOrDefault() ?? string.Empty,

                NextOffer1 = offer1?.Price ?? 0,
                NextOffer2 = offer2?.Price ?? 0,
               
            };
        }
    }
}
