namespace MarketPrice.Domain;

public static class ApiRoutes
{
    public const string AUTH="auth";
    public const string AUTH_REGISTER = "auth/register";
    public const string AUTH_LOGIN = "auth/login";
    public const string AUTH_LOGOUT = "auth/logout";
    public const string AUTH_REFRESH_TOKEN = "auth/refreshToken";
}

public static class ApiControllers
{
    public const string ApplicationUsers = "ApplicationUsers";
}

public static class StringExtensions
{
    public static string AppendRoute(this string apiController, params string[] routes)
    {
        foreach (var route in routes)
        {
            apiController = $"{apiController}/{route}";
        }

        return apiController;

    }
}