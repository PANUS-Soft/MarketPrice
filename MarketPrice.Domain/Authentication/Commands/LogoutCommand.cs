namespace MarketPrice.Domain.Authentication.Commands
{
    public class LogoutCommand
    {
        public required string EmailAddress { get; set; }
    }
}
