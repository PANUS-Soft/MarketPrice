namespace MarketPrice.Domain;

public static class ApiRoutes
{
    // Application Users Routes
    public const string AUTH = "auth";
    public const string AUTH_LOGIN = "auth/login";
    public const string AUTH_LOGOUT = "auth/logout";
    public const string AUTH_REGISTER = "auth/register";
    public const string AUTH_REFRESH_TOKEN = "auth/refreshToken";
    public const string AUTH_PING = "auth/ping";
    public const string GET_USER_PROFILE = "profile/get"; // "profile/get/{id}
    public const string UPDATE_USER_PROFILE = "profile/update";
    public const string CHANGE_PASSWORD = "changePassword";
    public const string GET_USER_ACTIVITY = "activities/get"; // "activity/get/{id}

    // Position Routes
    public const string BID_CREATE = "bid/create";
    public const string BID_UPDATE = "bid/update";
    public const string OFFER_CREATE = "offer/create";
    public const string OFFER_UPDATE = "offer/update";
    public const string POSITION_BY_PRICE = "price";
    public const string POSITION_DETAIL = "detail";

    // Reference Data Routes
    public const string REF_REGION = "regions";
    public const string REF_COMMODITY = "commodities";
    public const string REF_COMMODITY_TYPE = "commodityTypes";

    // Market Data Routes       
    public const string LOAD_MARKET_DATA = "market-data";
    public const string GET_MARKET_INSIGHT = "insight";
    public const string GET_CHART_DATA = "insight/{commodityId}/chart";

    // Home Data Routes
    public const string LOAD_HOME_DATA = "home-data";

    // Images Data Routes
    public const string LOAD_IMAGE = "{Id}/image";
}

public static class ApiControllers
{
    public const string ApplicationUsers = "ApplicationUsers";
    public const string Positions = "Positions";
    public const string ReferenceData = "ReferenceData";
    public const string Home = "Home";
    public const string CommodityTypeImages = "CommodityTypeImages";
    public const string CommodityImages = "CommodityImages";
    public const string Markets = "Markets";
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


