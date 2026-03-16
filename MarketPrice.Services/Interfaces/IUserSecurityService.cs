using MarketPrice.Domain.Profile.Command;
using MarketPrice.Domain.Profile.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Services.Interfaces
{
    public interface IUserSecurityService
    {
        Task<ChangePasswordResponseDto> ChangePasswordAsync(Guid UserId, ChangePasswordCommand command);
    }
}
