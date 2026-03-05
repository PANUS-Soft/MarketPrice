using MarketPrice.Domain.UserProfile.Command;
using MarketPrice.Domain.UserProfile.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileRespondDto> GetUserProfile(Guid UserId);

        Task UpdateUserProfile(Guid UserId, EditUserProfileCommand command);
    }
}
