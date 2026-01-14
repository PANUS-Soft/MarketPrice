namespace MarketPrice.Domain.Authentication.DTOs
{
    public class LoginResponseDto : AuthenticationResponseDto
    {
        public string FamilyName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
