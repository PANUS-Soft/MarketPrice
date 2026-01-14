namespace MarketPrice.Data.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public required int AccountTypeId { get; set; }
        public required string FirstName { get; set; }
        public required string FamilyName { get; set; }
        public string? OtherNames { get; set; }
        public string? IdCardNumber { get; set; }
        public required string EmailAddress { get; set; }
        public required string PhoneNumber { get; set; }
        public required bool IsPremiumUser { get; set; }
        public required string PasswordHash { get; set; }
        public required string PasswordSalt { get; set; }
        public required DateTimeOffset DateRecorded { get; set; } = DateTimeOffset.Now;
        public string? Note { get; set; }
        public DateTimeOffset? DateUpdate { get; set; }


    }
}
