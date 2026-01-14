namespace MarketPrice.Domain.Authentication.DTOs
{
    public class RegisterResponseDto : AuthenticationResponseDto
    {

    }



    public static class DtoManager
    {
        /// <summary>
        ///Returns a successful response
        /// </summary>
        /// <returns></returns>
        public static T Succeed<T>(T valueToReturn) where T : BaseResponseDto, new()
        {
            var response = valueToReturn;
            response.Success = true;
            return response;
        }

        /// <summary>
        /// if the user is already existing in the system
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>

        public static T Failed<T>(string status, params string[] errors) where T : BaseResponseDto, new()
        {
            var response = new T
            {
                Status = status,
                Errors = errors.ToArray()
            };
            return response;
        }

    }

    public abstract class BaseResponseDto
    {
        public string? Status { get; set; }
        public IEnumerable<string> Errors { get; set; } = [];

        public bool Success { get; set; }
    }
}

