using LinqToDB.Async;
using MarketPrice.Data;
using MarketPrice.Domain.Profile.Commands;
using MarketPrice.Domain.Profile.DTOs;
using MarketPrice.Services.Interfaces;

namespace MarketPrice.Services.Implementations;

public class ProfileService (MarketPriceDbContext context) : IProfileService
{
    public async Task<UserProfileResponseDto> GetUserProfileAsync(Guid id)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == id);

        if (user == null)
            return new UserProfileResponseDto
            {
                Status = false,
                Message = "User not found ... So can't load user profile."
            };

        var accountType = await context.LookupData.Where(at => at.LookupDataId == user.AccountTypeId).Select(at => at.LookupDataTextEnglish).FirstOrDefaultAsync() ?? "---";

        return new UserProfileResponseDto
        {
            UserId = user.UserId,
            FirstName = user.FirstName,
            FamilyName = user.FamilyName,
            OtherName = user.OtherNames ?? "",
            EmailAddress = user.EmailAddress,
            PhoneNumber = user.PhoneNumber,
            AccountType = accountType,
            Status = true,
            Message = "User profile data retrieved successfully."
        };
    }

    public async Task<UpdateUserProfileResponseDto> UpdateUserProfileAsync(UpdateUserProfileCommand command)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == command.UserId);

        var emailExists =
            await context.Users.AnyAsync(u => u.EmailAddress == command.EmailAddress && u.UserId != command.UserId);

        var phoneExists =
            await context.Users.AnyAsync(u => u.PhoneNumber == command.PhoneNumber && u.UserId != command.UserId);

        if (user == null)
            return new UpdateUserProfileResponseDto
            {
                Status = false,
                Message = "User not found so can't update profile."
            };

        if (command.EmailAddress != user.EmailAddress)
        {
            if (emailExists)
                return new UpdateUserProfileResponseDto
                {
                    Status = false,
                    Message = "Email or phone number already in use."
                };
        }

        if (command.PhoneNumber != user.PhoneNumber)
        {
            if (phoneExists)
                return new UpdateUserProfileResponseDto
                {
                    Status = false,
                    Message = "Email or phone number already in use."
                };
        }
        
        if (command.PhoneNumber != user.PhoneNumber && command.EmailAddress != user.EmailAddress)
        {
            if (phoneExists && emailExists)
                return new UpdateUserProfileResponseDto
                {
                    Status = false,
                    Message = "Email or phone number already in use."
                };
        }

        user.FirstName = command.FirstName ?? user.FirstName;
        user.FamilyName = command.FamilyName ?? user.FamilyName;
        user.OtherNames = command.OtherNames ?? user.OtherNames;
        user.EmailAddress = command.EmailAddress ?? user.EmailAddress;
        user.PhoneNumber = command.PhoneNumber ?? user.PhoneNumber;

        await context.SaveChangesAsync();

        return new UpdateUserProfileResponseDto
        {
            Status = true,
            Message = "User profile updated successfully."
        };
    }
}