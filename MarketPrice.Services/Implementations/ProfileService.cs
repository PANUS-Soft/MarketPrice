using LinqToDB.Async;
using MarketPrice.Data;
using MarketPrice.Domain.Authentication.DTOs;
using MarketPrice.Domain.Profile.Commands;
using MarketPrice.Domain.Profile.DTOs;
using MarketPrice.Services.Interfaces;

namespace MarketPrice.Services.Implementations;

public class ProfileService (MarketPriceDbContext context) : IProfileService
{
    public async Task<UserProfileResponseDto> GetUserProfileAsync(Guid id)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == id);

        if (user == null) return DtoManager.Failed<UserProfileResponseDto>("User not found ... So can't load user profile.");
            
        var accountType = await context.LookupData.Where(at => at.LookupDataId == user.AccountTypeId).Select(at => at.LookupDataTextEnglish).FirstOrDefaultAsync() ?? "---";

        var response = new UserProfileResponseDto
        {
            UserId = user.UserId,
            FirstName = user.FirstName,
            FamilyName = user.FamilyName,
            OtherName = user.OtherNames ?? "",
            EmailAddress = user.EmailAddress,
            Bio = user.Note,
            PhoneNumber = user.PhoneNumber,
            AccountType = accountType,
            Status = "User profile data retrieved successfully."
        };

        return DtoManager.Succeed(response);
    }

    public async Task<UpdateUserProfileResponseDto> UpdateUserProfileAsync(UpdateUserProfileCommand command)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == command.UserId);

        
        var emailExists =
            await context.Users.AnyAsync(u => u.EmailAddress == command.EmailAddress && u.UserId != command.UserId);

        var phoneExists =
            await context.Users.AnyAsync(u => u.PhoneNumber == command.PhoneNumber && u.UserId != command.UserId);

        if (user == null)
            return DtoManager.Failed<UpdateUserProfileResponseDto>("User not found so can't update profile.");

        if (command.EmailAddress != null && (command.EmailAddress != user.EmailAddress))
        {
            if (emailExists)
                return DtoManager.Failed<UpdateUserProfileResponseDto>("Email or phone number already in use.");
        }

        if (command.PhoneNumber != null && (command.PhoneNumber != user.PhoneNumber))
        {
            if (phoneExists)
                return DtoManager.Failed<UpdateUserProfileResponseDto>("Email or phone number already in use.");
        }

        var isEmailUpdated =
            !string.IsNullOrWhiteSpace(command.EmailAddress) &&
            command.EmailAddress != user.EmailAddress;

        var isPhoneUpdated =
            !string.IsNullOrWhiteSpace(command.PhoneNumber) &&
            command.PhoneNumber != user.PhoneNumber;

        if (isEmailUpdated && emailExists)
        {
            return DtoManager.Failed<UpdateUserProfileResponseDto>(
                "Email Address or Phone Number already in use.");
        }

        if (isPhoneUpdated && phoneExists)
        {
            return DtoManager.Failed<UpdateUserProfileResponseDto>(
                "Email Address or Phone number already in use.");
        }

        user.FirstName = command.FirstName ?? user.FirstName;
        user.FamilyName = command.FamilyName ?? user.FamilyName;
        user.OtherNames = command.OtherNames ?? user.OtherNames;
        user.Note = command.Bio ?? user.Note;
        user.EmailAddress = command.EmailAddress ?? user.EmailAddress;
        user.PhoneNumber = command.PhoneNumber ?? user.PhoneNumber;

        await context.SaveChangesAsync();

        return DtoManager.Succeed(new UpdateUserProfileResponseDto());
    }
}