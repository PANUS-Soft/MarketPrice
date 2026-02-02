using MarketPrice.Data;
using MarketPrice.Services.Interfaces;
using MarketPrice.Domain.Position;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MarketPrice.Data.Models;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Domain.Position.DTOs;
using Microsoft.EntityFrameworkCore;
using MarketPrice.Domain.Authentication.DTOs;



public class PositionService : IPositionService
{
    private readonly MarketPriceDbContext _context;
    private readonly ILookupProviderService _lookups;

    // Define the Type IDs as per your LookupDataTypes table
    private const int LOCATION_TYPE = 4000;
    private const int POSITION_STATUS = 5000;
    private const int POSITION_TYPE = 6000;
    private const int OPEN_POSITION = 5001;

    public PositionService(MarketPriceDbContext context, ILookupProviderService lookups)
    {
        _context = context;
        _lookups = lookups;
    }

    public async Task<PositionResponseDto> ProcessPositionAsync(PositionCommand command, bool isOffer)
    {
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
        try {
           statusId = _lookups.GetLookupId(statusText, POSITION_STATUS);
        } catch (Exception ex){
            throw new InvalidOperationException(
                $"Position status lookup failed for '{statusText}'", ex);
        }

        // 2. Fetch Position Type ID dynamically
        string posTypeText = isOffer ? "Offer" : "Bid";
        int posTypeId;
        try
        {
            posTypeId = _lookups.GetLookupId(posTypeText, POSITION_TYPE);
        }catch(Exception ex)
        {
            throw new InvalidOperationException(
                $"Position Type lookup failed for '{posTypeText}'", ex);
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
        int originLookupId = _lookups.GetLookupId("MainAddress", LOCATION_TYPE);
        var originLocation = MapToLocationEntity(command.Origin, command.UserId, originLookupId);
        _context.Locations.Add(originLocation);

        // 5. Create DeliveryDetail (Linked to Origin LocationId)
        var delivery = new DeliveryDetail
        {
            DeliveryDetailId = Guid.NewGuid(),
            PositionId = position.PositionId,
            LocationId = originLocation.LocationId,
            IsDeliverable = isOffer,
            LeadTime = isOffer ? (string?)command.LeadTime : null,
            Fee = isOffer ? (int?)command.DeliveryFee : 0,
            MaxDistance = isOffer ? 100 : 0
        };
        _context.DeliveryDetails.Add(delivery);

        // 6. Handle Destination
        if (isOffer && command.Destination != null)
        {
            int destLookupId = _lookups.GetLookupId("OtherAddress", LOCATION_TYPE);
            var destination = MapToLocationEntity(command.Destination, command.UserId, destLookupId);
            _context.Locations.Add(destination);
        }

        await _context.SaveChangesAsync();
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

    public async Task<PositionListingPageResponseDto> GetPositionListingsAsync(
    PositionListingCommand command)
    {
        try
        {
            if (command.UnitPrice <= 0)
            {
                return DtoManager.Failed<PositionListingPageResponseDto>(
                    "Invalid Criteria", "Unit price must be greater than zero.");
            }

            // 1. Fetch the Category (CommodityType) and ALL commodities under it
            // This ensures that when you switch types, you get the list of filterable items
            var categoryInfo = await _context.CommodityTypes
                .Where(ct => ct.CommodityTypeId == command.CommodityTypeId)
                .Select(ct => new {
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
                return DtoManager.Failed<PositionListingPageResponseDto>("NotFound", "Category not found.");
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
                .Select(p => new PositionListingResponseDto
                {
                    UserName = p.User.FirstName + " " + p.User.FamilyName,
                    CommodityName = p.Commodity.CommodityName,
                    Quantity = p.Quantity,
                    UnitOfMeasure = p.Commodity.UnitOfMeasure.UnitOfMeasureCodeEnglish
                })
                .ToListAsync();

            // 3. Compose Response
            return DtoManager.Succeed(new PositionListingPageResponseDto
            {
                CommodityTypeName = categoryInfo.TypeName,
                PositionTypeName = positionTypeName,
                CommodityNames = categoryInfo.AllCommodities, // All items for the filter UI
                Listings = listings, // Filtered result set
                UnitPrice = command.UnitPrice,
                Status = "Data Retrieved"
            });
        }
        catch (Exception ex)
        {
            return DtoManager.Failed<PositionListingPageResponseDto>("Error", "Retrieval failed.", ex.Message);
        }
    }
}
