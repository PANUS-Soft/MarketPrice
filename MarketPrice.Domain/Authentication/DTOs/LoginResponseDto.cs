namespace MarketPrice.Domain.Authentication.DTOs
{
    public class LoginResponseDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string FamilyName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string LoginStatus { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }

    }
}
