using LinqToDB.Async;
using MarketPrice.Data;
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
}