namespace MarketPrice.Domain.Authentication.Commands
{
    public class LoginCommand
    {
        public DateTimeOffset LoginDate { get; set; }
        public required string EmailAddress { get; set; }
        public required string Password { get; set; }
        public bool RememberMe { get; set; }

    }
}
