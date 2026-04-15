using LinqToDB.Internal.Linq;
using MarketPrice.Data;
using MarketPrice.Data.Models;
using MarketPrice.Domain.Activity.Command;
using MarketPrice.Domain.Activity.Dto;
using MarketPrice.Domain.Authentication.DTOs;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Domain.Position.DTOs;
using MarketPrice.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MarketPrice.Services.Implementations;

public class PositionService(MarketPriceDbContext context, ILookupProviderService lookups, IMarketRealtimeService realtime) : IPositionService
{
    private readonly MarketPriceDbContext _context = context;
    private readonly IMarketRealtimeService _realtime = realtime;

    // Define the Type IDs as per your LookupDataTypes table
    private const int LOCATION_TYPE = 4000;
    private const int POSITION_STATUS = 5000;
    private const int POSITION_TYPE = 6000;
    private const int OPEN_POSITION = 5001;

    public async Task<PositionResponseDto> ProcessPositionAsync(PositionCommand command, bool isOffer)
    {
        //verify if the command is null before process the data 
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        // 1. Determine Status using Dynamic Lookups
        if (command.EndDate <= command.StartDate)
            throw new ArgumentException("EndDate must be after StartDate");
        bool isOpen = DateTime.UtcNow >= command.StartDate && DateTime.UtcNow <= command.EndDate;

        if (command.UnitPrice <= 0)
            throw new ArgumentOutOfRangeException(nameof(command.UnitPrice));

        if (command.Quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(command.Quantity));

        string statusText = isOpen ? "Open" : "Close";
        int statusId;
        try
        {
            statusId = lookups.GetLookupId(statusText, POSITION_STATUS);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Position status lookup failed for '{statusText}'", ex);
        }

        // 2. Fetch Position Type ID dynamically
        string posTypeText = isOffer ? "Offer" : "Bid";
        int posTypeId;
        try
        {
            posTypeId = lookups.GetLookupId(posTypeText, POSITION_TYPE);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Position Type lookup failed for '{posTypeText}'", ex);
        }


        // 3. Map to Position Entity
        var position = new Position
        {
            PositionId = Guid.NewGuid(),
            UserId = command.UserId,
            CommodityId = command.CommodityId,
            PositionTypeId = posTypeId,
            CurrentStatusId = statusId,
            UnitPrice = command.UnitPrice,
            Quantity = command.Quantity,
            Grade = command.Grade,
            Description = command.Description,
            StartDate = command.StartDate,
            ExpiryDate = command.EndDate,
            Date = DateTime.UtcNow
        };

        _context.Positions.Add(position);

        // 4. Create Origin Location
        int originLookupId = lookups.GetLookupId("MainAddress", LOCATION_TYPE);
        var originLocation = MapToLocationEntity(command.Origin, command.UserId, originLookupId);
        _context.Locations.Add(originLocation);

        // 5. Handle Destination
        int destinationLookupId = lookups.GetLookupId("OtherAddress", LOCATION_TYPE);
        var destinationLocation = isOffer && command.Destination != null
            ? MapToLocationEntity(command.Destination, command.UserId, destinationLookupId)
            : null;
        if (destinationLocation != null) _context.Locations.Add(destinationLocation);

        // 6. Create DeliveryDetail (Linking Origin & Destination LocationId)
        var delivery = new DeliveryDetail
        {
            DeliveryDetailId = Guid.NewGuid(),
            PositionId = position.PositionId,
            OriginLocationId = originLocation.LocationId,
            DestinationLocationId = destinationLocation?.LocationId,
            IsDeliverable = command.CanDeliver,
            LeadTimeInDays = isOffer ? (string?)command.LeadTime : null,
            Fee = isOffer ? (int?)command.DeliveryFee : null,
            MaxDistance = null
        };
        _context.DeliveryDetails.Add(delivery);

        // 6. Handle Destination
        if (isOffer && command.Destination != null)
        {
            int destLookupId = lookups.GetLookupId("OtherAddress", LOCATION_TYPE);
            var destination = MapToLocationEntity(command.Destination, command.UserId, destLookupId);
            _context.Locations.Add(destination);
        }

        string state = DateTime.UtcNow < position.StartDate ? "Pending" :
            DateTime.UtcNow <= position.ExpiryDate ? "Open" : "Close";

        await _context.SaveChangesAsync();
        await _realtime.BroadcastPositionUpdateAsync(position, isOffer);
        await _realtime.BroadcastActivityPositionStatusUpdateAsync(position,state);

        //provide information to the grave curve
        return new PositionResponseDto
        {
            PositionId = position.PositionId,
            Message = isOffer ? "Offer successfully placed" : "Bid successfully placed",
            StatusId = statusId
        };
    }

    private Location MapToLocationEntity(LocationCommand cmd, Guid userId, int locationTypeId)
    {
        return new Location
        {
            LocationId = Guid.NewGuid(),
            UserId = userId,
            LocationTypeId = locationTypeId,
            RegionId = cmd.RegionId,
            Town = cmd.Town,
            Quarter = cmd.Quarter,
            Street = cmd.Street
        };
    }

    public async Task<PositionListingResponseDto> GetPositionListingsAsync(PositionListingCommand command)
    {
        try
        {
            if (command.UnitPrice <= 0)
            {
                return DtoManager.Failed<PositionListingResponseDto>(
                    "Invalid Criteria", "Unit price must be greater than zero.");
            }

            // 1. Fetch the Category (CommodityType) and ALL commodities under it
            // This ensures that when you switch types, you get the list of filterable items
            var categoryInfo = await _context.CommodityTypes
                .Where(ct => ct.CommodityTypeId == command.CommodityTypeId)
                .Select(ct => new
                {
                    TypeName = ct.Name.LookupDataTextEnglish,
                    // Get list of all commodity names for your dropdown/filter UI
                    AllCommodities = _context.Commodities
                        .Where(c => c.CommodityTypeId == ct.CommodityTypeId)
                        .Select(c => c.CommodityName)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (categoryInfo == null)
            {
                return DtoManager.Failed<PositionListingResponseDto>("NotFound", "Category not found.");
            }

            var positionTypeName = await _context.LookupData
                .Where(ld => ld.LookupDataId == command.PositionTypeId)
                .Select(ld => ld.LookupDataTextEnglish)
                .FirstOrDefaultAsync() ?? "Unknown";

            // 2. Build the query for Listings
            var listingsQuery = _context.Positions.AsQueryable();

            // Always filter by the Type, Price, and TypeId
            listingsQuery = listingsQuery.Where(p =>
                p.Commodity.CommodityTypeId == command.CommodityTypeId &&
                p.PositionTypeId == command.PositionTypeId &&
                p.UnitPrice == command.UnitPrice &&
                p.StartDate <= DateTime.UtcNow && p.ExpiryDate > DateTime.UtcNow);

            // OPTIONAL FILTER: If a specific CommodityId is passed, filter further.
            // If it is null/empty, it includes EVERYTHING under the CommodityType.
            if (command.CommodityId != null && command.CommodityId != Guid.Empty)
            {
                listingsQuery = listingsQuery.Where(p => p.CommodityId == command.CommodityId);
            }

            var listings = await listingsQuery
                .Select(p => new PositionListing
                {
                    PositionId = p.PositionId,
                    UserName = p.User.FirstName + " " + p.User.FamilyName,
                    CommodityName = p.Commodity.CommodityName,
                    Quantity = p.Quantity * (decimal)p.Commodity.LotSize!,
                    UnitOfMeasure = p.Commodity.UnitOfMeasure.UnitOfMeasureCodeEnglish
                })
                .ToListAsync();

            var ls = _context.Commodities
                .Where(c => c.CommodityTypeId == command.CommodityTypeId)
                .Select(c => c.LotSize)
                .FirstOrDefault();

            var uom = _context.Commodities
                .Where(c => c.CommodityTypeId == command.CommodityTypeId)
                .Select(c => c.UnitOfMeasure.UnitOfMeasureCodeEnglish)
                .FirstOrDefault();

            // 3. Compose Response
            return DtoManager.Succeed(new PositionListingResponseDto
            {
                CommodityTypeName = categoryInfo.TypeName,
                PositionTypeName = positionTypeName,
                CommodityNames = categoryInfo.AllCommodities, // All items for the filter UI
                Listings = listings, // Filtered result set
                UnitPrice = command.UnitPrice,
                LotSize = $"{ls} {uom}",
                Status = "Data Retrieved"
            });
        }
        catch (Exception ex)
        {
            return DtoManager.Failed<PositionListingResponseDto>("Error", "Retrieval failed.", ex.Message);
        }
    }

    public async Task<PositionDetailResponseDto> GetPositionDetailAsync(Guid id)
    {
        // Load position with the navigations that actually exist (User, Commodity)
        var position = await _context.Positions
            .AsNoTracking()
            .Include(p => p.User)
            .Include(p => p.Commodity).ThenInclude(commodity => commodity.UnitOfMeasure)
            .FirstOrDefaultAsync(p => p.PositionId == id);

        if (position == null)
            throw new KeyNotFoundException("Position not found");

        // Account type lookup
        var accountType = position.User != null ? await _context.LookupData
            .Where(ld => ld.LookupDataId == position.User.AccountTypeId)
            .Select(ld => ld.LookupDataValue)
            .FirstOrDefaultAsync() ?? string.Empty : string.Empty;

        // Grade is stored on the position table (assumes Position has GradeId)
        var grade = position.Grade ?? string.Empty;

        // Checking deliverable details
        var deliverable = await _context.DeliveryDetails
            .Where(dd => dd.PositionId == position.PositionId)
            .Select(dd => dd.IsDeliverable)
            .FirstOrDefaultAsync();

        // Delivery details are stored in a separate table - load by PositionId
        var deliveryDetail = await _context.DeliveryDetails
            .AsNoTracking()
            .FirstOrDefaultAsync(dd => dd.PositionId == position.PositionId);

        var originLocation = deliveryDetail != null
            ? await _context.Locations
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.LocationId == deliveryDetail.OriginLocationId)
            : null;

        var destinationLocation = (deliverable && deliveryDetail != null)
            ? await _context.Locations
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.LocationId == deliveryDetail.DestinationLocationId)
            : null;

        // Commodity type is stored separately; Commodity has CommodityTypeId
        var commodityType = position.Commodity != null
            ? await _context.CommodityTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(ct => ct.CommodityTypeId == position.Commodity.CommodityTypeId)
            : null;

        var commodityTypeName = position.Commodity != null
            ? await _context.LookupData
                .Where(ld => ld.LookupDataId == commodityType.NameId)
                .Select(ld => ld.LookupDataTextEnglish)
                .FirstOrDefaultAsync() : string.Empty;

        var uom = position.Commodity != null
            ? await _context.UnitOfMeasures
                .Where(uom => uom.UnitOfMeasureId == position.Commodity.UnitOfMeasureId)
                .Select(uom => uom.UnitOfMeasureCodeEnglish)
                .FirstOrDefaultAsync() : string.Empty;

        // Map defensively
        return new PositionDetailResponseDto
        {
            // User information
            UserId = position.UserId,
            UserName = position.User != null ? $"{position.User.FirstName} {position.User.FamilyName}".Trim() : string.Empty,
            AccountType = accountType,
            PhoneNumber = position.User != null ? position.User.PhoneNumber : string.Empty,

            // Commodity information
            CommodityName = position.Commodity != null ? position.Commodity.CommodityName : string.Empty,
            CommodityTypeName = commodityTypeName ?? string.Empty,
            CommodityCode = commodityType != null ? commodityType.Code : string.Empty,
            Grade = grade,
            UnitOfMeasure = uom ?? string.Empty,

            // Position information
            Quantity = position.Quantity,
            UnitPrice = position.UnitPrice,
            LotSize = position.Commodity != null ? position.Commodity.LotSize : 0,
            ShelfLifeInDays = position.Commodity != null ? position.Commodity.ShelfLifeInDays : 0,
            DeliveryAvailable = deliverable,

            // Logistics infor mation
            Origin = originLocation != null
                ? new LocationResponse
                {
                    Region = originLocation.RegionId != null ? await _context.LookupData
                        .Where(ld => ld.LookupDataId == originLocation.RegionId)
                        .Select(ld => ld.LookupDataTextEnglish)
                        .FirstOrDefaultAsync() : null,
                    Town = originLocation.Town,
                    Quarter = originLocation.Quarter,
                    Street = originLocation.Street
                } : null,
            Destination = destinationLocation != null ? new LocationResponse
            {
                Region = destinationLocation.RegionId != null ? await _context.LookupData
                    .Where(ld => ld.LookupDataId == destinationLocation.RegionId)
                    .Select(ld => ld.LookupDataTextEnglish)
                    .FirstOrDefaultAsync() : null,
                Town = destinationLocation.Town,
                Quarter = destinationLocation.Quarter,
                Street = destinationLocation.Street
            } : null,
            LeadTimeInDays = deliverable ? deliveryDetail?.LeadTimeInDays : null,
            DeliveryFee = deliverable ? deliveryDetail?.Fee : null
        };
    }
    public async Task<ActivityGroupDto> GetActivityAsync(ActivityCommand command)
    {
        var positionHistory = _context.Positions.AsQueryable();

        //Filter: Position type (SAFE using Lookup IDs)
        if(!string.IsNullOrEmpty(command.PositionType))
        {
            int typeId = command.PositionType switch
            {
                "Bid" => lookups.GetLookupId("Bid", POSITION_TYPE),
                "Offer" => lookups.GetLookupId("Offer", POSITION_TYPE),
                _ => throw new ArgumentException($"Unknow PositionType: {command.PositionType}")
            };
        positionHistory = positionHistory.Where(p => p.PositionTypeId == typeId);

        }
        //Filter: Commodity
        if(command.CommodityId.HasValue && command.CommodityId != Guid.Empty)
        {
            positionHistory = positionHistory.Where(p => p.CommodityId == command.CommodityId.Value);
        }

        //limit Data (Performance)
        //last week position placement
        var LastWeek = DateTime.UtcNow.AddDays(-7);
        positionHistory = positionHistory.Where(p => p.Date >= LastWeek);

        var data = await positionHistory.Select(p => new ActivityResponseDto
        {
            CommodityName = p.Commodity.CommodityName,
            Quantity = p.Quantity,
            Price = p.UnitPrice,

            State = DateTime.UtcNow < p.StartDate? "Pending..":
            p.ExpiryDate >= DateTime.UtcNow ? "Open" : "Close",

            PositionType = p.PositionType.LookupDataTextEnglish,
            CreatedAt = p.Date

        }).OrderByDescending(X => X.CreatedAt).ToListAsync();

        //Grouping all them with they respective time range 

        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);
        var ThisWeek = DateTime.UtcNow.AddDays(7);


        return new ActivityGroupDto
        {
            Today = data.Where(x => x.CreatedAt >= today).ToList(),
            Yesterday = data.Where(x => x.CreatedAt >= yesterday && x.CreatedAt < today).ToList(),
            LastWeek = data.Where(x => x.CreatedAt < yesterday).ToList(),
        };
    }
}