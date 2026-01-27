namespace MarketPrice.Domain;

public static class ApiRoutes
{
    // Authentication Routes
    public const string AUTH="auth";
    public const string AUTH_LOGIN = "auth/login";
    public const string AUTH_LOGOUT = "auth/logout";
    public const string AUTH_REGISTER = "auth/register";
    public const string AUTH_REFRESH_TOKEN = "auth/refreshToken";
    public const string AUTH_PING = "auth/ping";


    // Position Routes
    public const string BID_CREATE = "bid/create";
    public const string OFFER_CREATE = "offer/create";
    public const string POSITION_BY_PRICE = "bestPrice";

    // Reference Data Routes
    public const string REF_REGION = "regions";
    public const string REF_COMMODITY = "commodities";
    public const string REF_COMMODITY_TYPE = "commodityTypes";


    // Market Data Routes

    // Home Data Routes
    public const string HOME_DATA = "data";

    // Commoduity Type Images Route
    public const string IMAGE_DATA = "{commodityTypeId}/image";
}

public static class ApiControllers
{
    public const string ApplicationUsers = "ApplicationUsers";
    public const string Positions = "Positions";
    public const string ReferenceData = "ReferenceData";
    public const string Home = "Home";
    public const string CommodityTypeImages = "CommodityTypeImages";
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