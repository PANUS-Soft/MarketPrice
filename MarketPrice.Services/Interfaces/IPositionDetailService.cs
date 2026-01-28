using MarketPrice.Domain.Position.Commands;
using MarketPrice.Domain.Position.DTOs;
using System.Threading.Tasks;

namespace MarketPrice.Services.Interfaces
{
    public interface IPositionDetailService
    {
        Task<PositionDetailResponseDTO> GetPositionDetailAsync(PositionDetailCommand command);
    }
}