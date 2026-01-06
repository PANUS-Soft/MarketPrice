namespace MarketPrice.Domain.Authentication.DTOs
{
    public class RegisterResponseDto
    {

        public bool Success { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTimeOffset ExpiryDate { get; set; }
        public string CreationStatus { get; set; } = string.Empty;
        public IEnumerable<string> Errors { get; set; } = [];

        /// <summary>
        /// User successfuly register to the platform
        /// </summary>
        /// <param name="email"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static RegisterResponseDto Succeed(string email, string message, string firstName, string accessToken, string refreshToken, DateTimeOffset expiryDate)
        {
            return new RegisterResponseDto
            {
                EmailAddress = email,
                FirstName = firstName,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiryDate = expiryDate,
                CreationStatus = message,
                Success = true
            };
        }

        /// <summary>
        /// if the user is already existing in the system
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>

        public static RegisterResponseDto Failed(string errorMessage)
        {
            return new RegisterResponseDto
            {
                FirstName = string.Empty,
                AccessToken = string.Empty,
                RefreshToken = string.Empty,
                ExpiryDate = DateTimeOffset.MinValue,
                EmailAddress = string.Empty,
                CreationStatus = errorMessage,
                Success = false,
            };
        }
        /// <summary>
        /// A list of errors that occurred during registration.
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static RegisterResponseDto Failed(IEnumerable<string> error)
        {
            return new RegisterResponseDto
            {
                Success = false,
                Errors = error,
                CreationStatus = "Registration failed",

            };

        }
    }
}
