using MarketPrice.Services.Interfaces;
using MarketPrice.Data;
using MarketPrice.Domain.UserProfile.DTOs;
using MarketPrice.Domain.UserProfile.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MarketPrice.Services.Implementations
{
    public class UserProfileService : IUserProfileService
    {

        private readonly MarketPriceDbContext _context;

        public UserProfileService(MarketPriceDbContext context)
        {
            _context = context;
        }

        // View the user profile and then save to the database for better approche.     
        public async Task<UserProfileRespondDto> GetUserProfile(Guid UserId)
        {
            var dto = await (from u in _context.Users.AsNoTracking()
                             join ac in _context.LookupData.AsNoTracking() on u.AccountTypeId
                             equals ac.LookupDataId into accountGroup
                             from ac in accountGroup.DefaultIfEmpty()
                             where u.UserId == UserId
                             select new UserProfileRespondDto
                             {
                                 FirstName = u.FirstName,
                                 FamilyName = u.FamilyName,
                                 OtherName = u.OtherNames,
                                 EmailAddress = u.EmailAddress,
                                 PhoneNumber = u.PhoneNumber,
                                 AccountType = ac != null ? (ac.LookupDataValue ?? string.Empty) : string.Empty
                             }).FirstOrDefaultAsync();

            if (dto == null)
                throw new KeyNotFoundException("User not found");

            return dto;

        }
        // update the user profile and then save it to the database 
        public async Task UpdateUserProfile(Guid UserId, EditUserProfileCommand command)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == UserId);

            if (user == null) throw new KeyNotFoundException("User not found");

            user.FirstName = command.FirstName ?? user.FirstName; 
            user.FamilyName = command.FamilyName ?? user.FamilyName;
            user.OtherNames = command.OtherName ?? user.OtherNames;
            user.EmailAddress = command.EmailAddress ?? user.EmailAddress;
            user.PhoneNumber = command.PhoneNumber ?? user.PhoneNumber;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

        }


    }
}
