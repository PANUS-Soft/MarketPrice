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
        bool isOpen = DateTime.Now >= command.StartDate && DateTime.Now <= command.EndDate;

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
        string posTypeText = isOffer ? "Ask" : "Bid";
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
            Date = DateTime.Now
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

    public async Task<List<PositionListingResponseDto>> GetPositionsForPriceAsync(PositionListingCommand command)
    {
        return await _context.Positions
            .Where(p => p.Commodity.CommodityTypeId == command.CommodityTypeId
                     && p.PositionTypeId == command.PositionTypeId
                     && p.UnitPrice == command.UnitPrice
                     && p.CurrentStatusId == OPEN_POSITION)
            .Select(p => new PositionListingResponseDto
            {
                // We use the navigation property 'User' to get the name
                UserName = $"{p.User.FirstName} {p.User.FamilyName}",
                Quantity = p.Quantity,
                // We use the navigation property 'Commodity' to get the specific name (e.g., Yellow Corn)
                CommodityName = p.Commodity.CommodityName,
                UnitOfMeasure = p.Commodity.UnitOfMeasure.UnitOfMeasureCodeEnglish
            })
            .ToListAsync();
    }

}