namespace MarketPrice.Domain.Authentication.Commands
{
    public class RegisterCommand
    {
        public required string FirstName { get; set; }
        public required string FamilyName { get; set; }
        public string? OtherNames { get; set; }
        public required int AccountTypeId { get; set; }
        public required string EmailAddress { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }

    }



}
