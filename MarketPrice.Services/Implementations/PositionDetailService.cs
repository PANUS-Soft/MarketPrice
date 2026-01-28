using MarketPrice.Data;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using MarketPrice.Domain.Position.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketPrice.Services.Implementations
{
    public class PositionDetailService : IPositionDetailService
    {
        private readonly MarketPriceDbContext _context;
        public PositionDetailService(MarketPriceDbContext context)
        {
            _context = context;
        }

        public async Task<PositionDetailResponseDTO> GetPositionDetailAsync(PositionDetailCommand command)
        {
            // Load position with the navigations that actually exist (User, Commodity)
            var position = await _context.Positions
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Commodity)
                .FirstOrDefaultAsync(p => p.PositionId == command.PositionId);

            if (position == null)
                throw new KeyNotFoundException("Position not found");

            // account type lookup
            var accountType = await _context.LookupData
                .Where(ld => ld.LookupDataId == position.User.AccountTypeId)
                .Select(ld => ld.LookupDataValue)
                .FirstOrDefaultAsync() ?? string.Empty;

            // grade is stored on the position table (assumes Position has GradeId)
            var grade = position.Grade ?? string.Empty;

            // user location (town)
            var userLocation = await _context.Locations
                .Where(l => l.UserId == position.UserId)
                .Select(l => new { l.Town })
                .FirstOrDefaultAsync();

            // delivery details are stored in a separate table - load by PositionId
            var delivery = await _context.DeliveryDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(dd => dd.PositionId == position.PositionId);

            // destination location (if DeliveryDetail.LocationId points to Locations)
            var destinationLocation = delivery != null
                ? await _context.Locations.AsNoTracking().FirstOrDefaultAsync(l => l.LocationId == delivery.LocationId)
                : null;

            // commodity type is stored separately; Commodity has CommodityTypeId
            var commodityType = position.Commodity != null
                ? await _context.CommodityTypes.AsNoTracking()
                    .FirstOrDefaultAsync(ct => ct.CommodityTypeId == position.Commodity.CommodityTypeId)
                : null;

            // Map defensively
            return new PositionDetailResponseDTO
            {
                // Seller
                UserId = position.UserId,
                UserName = position.User == null
                    ? string.Empty
                :$"{(position.User.FirstName ?? string.Empty).ToUpper()} {(position.User.FamilyName ?? string.Empty).ToUpper()}".Trim(),
                AccountType = accountType,

                // Individual
                Location = userLocation?.Town ?? string.Empty,

                // Commodity
                CommodityName = position.Commodity?.CommodityName ?? string.Empty,
                // Code is on CommodityType in your model; fall back to empty if not available
                CommodityCode = commodityType?.Code ?? string.Empty,
                Grade = grade,

                // Position
                Quantity = position.Quantity,
                UnitPrice = position.UnitPrice,
                LotSize = position.Commodity?.LotSize ?? 0,
                ShelfLife = position.Commodity?.ShelfLifeInDays?.ToString() ?? string.Empty,

                // Delivery
                Origin = userLocation?.Town ?? string.Empty,
                Destination = destinationLocation?.Town ?? string.Empty,
                LeadTimeDays = int.TryParse(delivery?.LeadTime, out var days) ? days : 0,
                DeliveryFee = delivery?.Fee ?? 0m,
                DeliveryAvailable = delivery?.IsDeliverable ?? false,

                // Contact
                PhoneNumber = position.User?.PhoneNumber ?? string.Empty
            };
        }
    }
}