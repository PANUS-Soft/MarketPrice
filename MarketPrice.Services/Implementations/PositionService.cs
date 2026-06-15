using LinqToDB.Internal.Linq;
using MarketPrice.Data;
using MarketPrice.Data.Models;
using MarketPrice.Domain.Activity.DTOs;
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
    private const int REGIONS = 7000;
    private const int OPEN_POSITION = 5001;

    public async Task<PositionResponseDto> ProcessPositionAsync(CreatePositionCommand command, bool isOffer)
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

    public async Task<UpdatePositionResponseDto> UpdatePositionAsync(UpdatePositionCommand command, bool isOffer)
    {
        // Implementation for updating a position
        var position = await _context.Positions.FirstOrDefaultAsync(p => p.PositionId == command.PositionId && p.UserId == command.UserId);
        
        if (position == null)
        {
            return DtoManager.Failed<UpdatePositionResponseDto>("Not Found", "Position not found. There was an error in fetching the position. Invalid PositionId or UserId.");
        }

        var deliveryDetail = await _context.DeliveryDetails.FirstOrDefaultAsync(dd => dd.PositionId == position.PositionId);

        if (deliveryDetail == null)
        {
            return DtoManager.Failed<UpdatePositionResponseDto>("Not Found", "Delivery details not found.");
        }

        var originLocation = await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == deliveryDetail.OriginLocationId);

        Location? destinationLocation = null;

        if (deliveryDetail.DestinationLocationId != null)
        {
            destinationLocation = await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == deliveryDetail.DestinationLocationId);
        }

        // Update the position properties
        position.Grade = command.Grade;
        position.Quantity = command.Quantity;
        position.UnitPrice = command.UnitPrice;
        position.Description = command.Description;
        position.ExpiryDate = command.EndDate;

        // Update the delivery details
        // Update the origin location
         if (originLocation != null)
         {
            originLocation.RegionId = command.Origin.RegionId;
            originLocation.Town = command.Origin.Town;
            originLocation.Quarter = command.Origin.Quarter;
            originLocation.Street = command.Origin.Street;
         }
        
         // Update the destination location if it exists and delivery is available
         if (isOffer && command.CanDeliver)
         {
             deliveryDetail.IsDeliverable = true;
             deliveryDetail.LeadTimeInDays = command.LeadTime;
             deliveryDetail.Fee = command.DeliveryFee;

             if (destinationLocation == null) 
             {
                int destinationLookupId = lookups.GetLookupId("OtherAddress", LOCATION_TYPE);

                destinationLocation = MapToLocationEntity(command.Destination!, command.UserId, destinationLookupId);

                _context.Locations.Add(destinationLocation);

                deliveryDetail.DestinationLocationId = destinationLocation.LocationId;
             }
             else
             {
                destinationLocation.RegionId = command.Destination!.RegionId;
                destinationLocation.Town = command.Destination.Town;
                destinationLocation.Quarter = command.Destination.Quarter;
                destinationLocation.Street = command.Destination.Street;
             }
         }
         else
         {
             deliveryDetail.IsDeliverable = false;
             deliveryDetail.LeadTimeInDays = null;
             deliveryDetail.Fee = null;

             if (destinationLocation != null)
             {
                _context.Locations.Remove(destinationLocation);
                deliveryDetail.DestinationLocationId = null;
             }
         }

         position.DateUpdated = DateTime.UtcNow;

         await _context.SaveChangesAsync();
         await _realtime.BroadcastPositionUpdateAsync(position, isOffer);
         
         var dto = new UpdatePositionResponseDto
         {
             Status = "Position updated successfully."
         };
         
         return DtoManager.Succeed<UpdatePositionResponseDto>(dto);
    }

    public async Task<PositionListingResponseDto> GetPositionListingsAsync(PositionListingCommand command)
    {
        try
        {
            var now = DateTime.UtcNow;
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
                p.StartDate <= now && p.ExpiryDate > now);

            // OPTIONAL FILTER: If a specific CommodityId is passed, filter further.
            // If it is null/empty, it includes EVERYTHING under the CommodityType.
            if (command.CommodityId != null && command.CommodityId != Guid.Empty)
            {
                listingsQuery = listingsQuery.Where(p => p.CommodityId == command.CommodityId);
            }

            // --- NEW: First pull raw data so EF Core can translate the sub-queries safely ---
            var rawListings = await listingsQuery
                .Select(p => new
                {
                    p.PositionId,
                    UserName = p.User.FirstName + " " + p.User.FamilyName,
                    CommodityName = p.Commodity.CommodityName,
                    CalculatedQuantity = p.Quantity * (decimal)p.Commodity.LotSize!,
                    UoM = p.Commodity.UnitOfMeasure.UnitOfMeasureCodeEnglish,
                    ShelfLifeInDays = p.Commodity.ShelfLifeInDays,
                    BaseQuantity = p.Quantity,
                    p.UnitPrice,
                    p.StartDate,
                    p.ExpiryDate,
                    // Sub-query to fetch the town
                    Town = _context.DeliveryDetails
                        .Where(dd => dd.PositionId == p.PositionId)
                        .Select(dd => _context.Locations.Where(l => l.LocationId == dd.OriginLocationId).Select(l => l.Town).FirstOrDefault())
                        .FirstOrDefault(),

                    // NEW: Sub-query to fetch the Region Name (LocationName)
                    RegionName = _context.DeliveryDetails
                        .Where(dd => dd.PositionId == p.PositionId)
                        .Select(dd => _context.Locations
                            .Where(l => l.LocationId == dd.OriginLocationId)
                            .Select(l => _context.LookupData.Where(ld => ld.LookupDataId == l.RegionId).Select(ld => ld.LookupDataTextEnglish).FirstOrDefault())
                            .FirstOrDefault())
                        .FirstOrDefault(),

                    IsDeliverable = _context.DeliveryDetails
                        .Where(dd => dd.PositionId == p.PositionId)
                        .Select(dd => dd.IsDeliverable)
                        .FirstOrDefault(),
                    Fee = _context.DeliveryDetails
                        .Where(dd => dd.PositionId == p.PositionId)
                        .Select(dd => dd.Fee)
                        .FirstOrDefault()
                })
                .ToListAsync();

            //var now = DateTimeOffset.UtcNow;

            // Map the raw data to the final DTO
            var listings = rawListings.Select(p =>
            {
                var timeToExpiry = p.ExpiryDate - now;
                int daysLeft = Math.Max(0, (int)Math.Ceiling(timeToExpiry.TotalDays));

                return new PositionListing
                {
                    PositionId = p.PositionId,
                    UserName = p.UserName,
                    CommodityName = p.CommodityName,
                    Quantity = p.CalculatedQuantity,
                    UnitOfMeasure = p.UoM,
                    ShelfLifeInDays = p.ShelfLifeInDays,
                    OriginTown = p.Town ?? "Unknown",

                    // NEW: Map the Region Name so the UI filter can read it!
                    LocationName = p.RegionName,

                    TotalPrice = p.BaseQuantity * p.UnitPrice,
                    IsExpiringSoon = IsExpired(p.StartDate, p.ExpiryDate, now),
                    ExpiryText = daysLeft <= 1 ? "Expiring in 1 day" : $"Expiring in {daysLeft} days",
                    IsCriticalExpiry = daysLeft <= 1,
                    IsDeliverable = p.IsDeliverable,
                    DeliveryFee = p.Fee.HasValue ? (decimal?)p.Fee.Value : null
                };
            }).ToList();

            var ls = _context.Commodities
                .Where(c => c.CommodityTypeId == command.CommodityTypeId)
                .Select(c => new { c.LotSize, c.UnitOfMeasure.UnitOfMeasureCodeEnglish})
                .FirstOrDefault();

            

            var shelfLife = _context.Commodities
                .Where(c => c.CommodityTypeId == command.CommodityTypeId)
                .Select(c => c.ShelfLifeInDays)
                .FirstOrDefault();

            // FETCH THE REAL PRICE LADDERS AND PEOPLE COUNTS 
            var ladderBaseQuery = _context.Positions.Where(p =>
                p.Commodity.CommodityTypeId == command.CommodityTypeId &&
                p.StartDate <= DateTime.UtcNow && p.ExpiryDate > DateTime.UtcNow);

            if (command.CommodityId != null && command.CommodityId != Guid.Empty)
            {
                ladderBaseQuery = ladderBaseQuery.Where(p => p.CommodityId == command.CommodityId);
            }

            // Bids are sorted Highest to Lowest
            var bidPrices = await ladderBaseQuery
                .Where(p => p.PositionTypeId == 6001)
                .GroupBy(p => p.UnitPrice)
                .Select(g => new PricePointDto
                {
                    Price = g.Key,
                    Count = g.Select(p => p.UserId).Distinct().Count() // Number of distinct people
                })
                .OrderByDescending(p => p.Price)
                .ToListAsync();

            // Offers are sorted Lowest to Highest
            var offerPrices = await ladderBaseQuery
                .Where(p => p.PositionTypeId == 6002)
                .GroupBy(p => p.UnitPrice)
                .Select(g => new PricePointDto
                {
                    Price = g.Key,
                    Count = g.Select(p => p.UserId).Distinct().Count() // Number of distinct people
                })
                .OrderBy(p => p.Price)
                .ToListAsync();

            // 3. Compose Response
            return DtoManager.Succeed(new PositionListingResponseDto
            {
                CommodityTypeName = categoryInfo.TypeName,
                PositionTypeName = positionTypeName,
                CommodityNames = categoryInfo.AllCommodities,
                Listings = listings,
                UnitPrice = command.UnitPrice,
                LotSize = $"{ls}",
                ShelfLife = shelfLife != null ? $"{shelfLife} Days" : "---",

                // ADD THE NEW LISTS HERE
                BidPrices = bidPrices,
                OfferPrices = offerPrices,

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

    public async Task<ActivityGroupDto> GetActivityAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required.");

        var now = DateTime.UtcNow;

        var positions = await _context.Positions.AsNoTracking().Where(p => p.UserId == userId).Include(p => p.Commodity)
            .ThenInclude(c => c.UnitOfMeasure).OrderByDescending(p => p.Date).ToListAsync();

        if (!positions.Any()) return new ActivityGroupDto();

        var positionIds = positions.Select(p => p.PositionId).ToList();

        var deliveryDetails = await _context.DeliveryDetails.AsNoTracking()
            .Where(dd => positionIds.Contains(dd.PositionId)).ToListAsync();

        var allLocationIds = deliveryDetails.Select(dd => dd.OriginLocationId)
            .Concat(deliveryDetails.Where(dd => dd.DestinationLocationId.HasValue)
                .Select(dd => dd.DestinationLocationId!.Value)).Distinct().ToList();

        var locationById = await _context.Locations.AsNoTracking().Where(l => allLocationIds.Contains(l.LocationId))
            .ToDictionaryAsync(l => l.LocationId);

        var allRegionIds = locationById.Values.Select(l => l.RegionId).Distinct().ToList();

        var regionNamesById = await _context.LookupData.AsNoTracking()
            .Where(ld => allRegionIds.Contains(ld.LookupDataId)).ToDictionaryAsync(ld => ld.LookupDataId, ld => ld.LookupDataTextEnglish);

        var deliveryByPositionId = deliveryDetails.ToDictionary(dd => dd.PositionId);
        var bidTypeId = lookups.GetLookupId("Bid", POSITION_TYPE);

        var data = positions.Select(p =>
        {
            var delivery = deliveryByPositionId.TryGetValue(p.PositionId, out var d) ? d : null;
            var isDeliverable = delivery?.IsDeliverable ?? false;

            var originLocation = delivery != null && locationById.TryGetValue(delivery.OriginLocationId, out var ol)
                ? ol
                : null;

            var destinationLocation =
                isDeliverable && delivery?.DestinationLocationId != null &&
                locationById.TryGetValue(delivery.DestinationLocationId.Value, out var dl)
                    ? dl
                    : null;

            return new ActivityResponseDto
            {
                PositionId = p.PositionId,
                CommodityId = p.CommodityId,
                CommodityTypeId = p.Commodity?.CommodityTypeId ?? Guid.Empty,
                CommodityName = p.Commodity?.CommodityName ?? string.Empty,
                ShelfLifeInDays = p.Commodity?.ShelfLifeInDays ?? 0,
                UnitOfMeasure = p.Commodity?.UnitOfMeasure?.UnitOfMeasureCodeEnglish ?? string.Empty,
                LotSize = p.Commodity?.LotSize,

                Quantity = p.Quantity,
                UnitPrice = p.UnitPrice,
                Grade = p.Grade ?? string.Empty,
                Description = p.Description,
                PositionType = p.PositionTypeId == bidTypeId ? "Bid" : "Offer",
                StartDate = p.StartDate,
                EndDate = p.ExpiryDate,
                CreatedAt = p.Date,
                State = now < p.StartDate ? "Pending" : p.ExpiryDate >= now ? "Open" : "Close",
                CanDeliver = isDeliverable,
                LeadTime = isDeliverable ? delivery?.LeadTimeInDays : null,
                DeliveryFee = isDeliverable ? delivery?.Fee : null,
                OriginRegion = originLocation != null && regionNamesById.TryGetValue(originLocation.RegionId, out var orName) ? orName : null,
                DestinationRegion = destinationLocation != null && regionNamesById.TryGetValue(destinationLocation.RegionId, out var drName) ? drName : null,

                Origin = originLocation != null
                    ? new LocationCommand
                    {
                        RegionId = originLocation.RegionId,
                        Town = originLocation.Town,
                        Quarter = originLocation.Quarter,
                        Street = originLocation.Street
                    }
                    : null,
                Destination = destinationLocation != null
                    ? new LocationCommand
                    {
                        RegionId = destinationLocation.RegionId,
                        Town = destinationLocation.Town,
                        Quarter = destinationLocation.Quarter,
                        Street = destinationLocation.Street
                    }
                    : null,
            };
        }).ToList();

        var today = now.Date;
        var yesterday = today.AddDays(-1);
        int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
        var startOfWeek = today.AddDays(-diff);
        var startOfLastWeek = startOfWeek.AddDays(-7);
        var endOfLastWeek = startOfWeek;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var startOfLastMonth = startOfMonth.AddMonths(-1);

        return new ActivityGroupDto
        {
            Today = data.Where(x => x.CreatedAt >= today).ToList(),

            Yesterday = data.Where(x => x.CreatedAt >= yesterday && x.CreatedAt < today).ToList(),

            ThisWeek = data.Where(x => x.CreatedAt >= startOfWeek && x.CreatedAt < yesterday).ToList(),

            LastWeek = data.Where(x => x.CreatedAt >= startOfLastWeek && x.CreatedAt < endOfLastWeek).ToList(),

            ThisMonth = data.Where(x => x.CreatedAt >= startOfMonth && x.CreatedAt < startOfLastWeek).ToList(),

            LastMonth = data.Where(x => x.CreatedAt >= startOfLastMonth && x.CreatedAt < startOfMonth).ToList(),

        };
    }

    public async Task<DeleteActivityResponseDto> DeleteActivityAsync(Guid positionId)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            if (positionId == Guid.Empty)
                throw new ArgumentException("PositionId is required.");

            var position = await _context.Positions.FirstOrDefaultAsync(p => p.PositionId == positionId);

            if (position == null)
                return DtoManager.Failed<DeleteActivityResponseDto>("Not Found", "Activity with PositionId not found.");

            // Getting the delivery details of the position
            var deliveryDetail = await _context.DeliveryDetails.FirstOrDefaultAsync(dd => dd.PositionId == positionId);

            if (deliveryDetail != null)
            {
                // Deleting the origin location
                var origin =
                    await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == deliveryDetail.OriginLocationId);

                if (origin != null)
                    _context.Locations.Remove(origin);

                // Deleting the destination location (if it exists)
                if (deliveryDetail.DestinationLocationId.HasValue)
                {
                    var destination = await _context.Locations.FirstOrDefaultAsync(l =>
                        l.LocationId == deliveryDetail.DestinationLocationId.Value);

                    if (destination != null)
                        _context.Locations.Remove(destination);
                }

                // Deleting delivery detail of the position
                _context.DeliveryDetails.Remove(deliveryDetail);
            }


            // Deleting the position entry
            _context.Positions.Remove(position);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return DtoManager.Succeed(new DeleteActivityResponseDto { Status = "Activity deleted successfully." });
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // FIX: Using DateTimeOffset instead of DateTime
    private bool IsExpired(DateTimeOffset start, DateTimeOffset expiry, DateTimeOffset now)
    {
        var total = (expiry - start).TotalSeconds;
        if (total <= 0) return false;
        return ((now - start).TotalSeconds / total) >= 0.8;
    }

}