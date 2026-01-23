using MarketPrice.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketPrice.Domain.Position.Commands;
using MarketPrice.Domain.Position.DTOs;


namespace MarketPrice.Services.Interfaces
{
    public interface IPositionService
    {
        Task<PositionResponseDto> ProcessPositionAsync(PositionCommand command, bool isOffer);

        Task<PositionListingPageResponseDto> GetPositionListingsAsync(PositionListingCommand command);
    }
}