namespace MarketPrice.Ui.Common;

public static class AuthenticationNavigation
{
    public const string RedirectToParameter = "redirectTo";

    private static readonly HashSet<string> AllowedDestinations = new(StringComparer.OrdinalIgnoreCase)
    {
        "//Activity",
        "//Profile",
        "//Home",
        "//Market",
        "//Market/MarketInsight",
        "//Market/PositionListing",
        "//Home/PositionListing"
    };

    public static Task NavigateToLoginAsync(string destination)
    {
        var route = ValidateDestination(destination)
            ?? throw new ArgumentException("The post-login destination is not allowed.", nameof(destination));

        return Shell.Current.GoToAsync(
            $"//Login?{RedirectToParameter}={Uri.EscapeDataString(route)}");
    }

    public static Task NavigateToWelcomeAsync(string destination)
    {
        var route = ValidateDestination(destination)
            ?? throw new ArgumentException("The post-login destination is not allowed.", nameof(destination));

        return Shell.Current.GoToAsync(
            $"//Welcome?{RedirectToParameter}={Uri.EscapeDataString(route)}");
    }

    public static string CurrentRouteOr(string fallback)
    {
        var currentRoute = Shell.Current.CurrentState.Location.OriginalString;
        return ValidateDestination(currentRoute) ?? fallback;
    }

    public static string? ReadDestination(IDictionary<string, object> query)
    {
        if (!query.TryGetValue(RedirectToParameter, out var value)) return null;

        var route = value?.ToString();
        if (string.IsNullOrWhiteSpace(route)) return null;

        try
        {
            return ValidateDestination(Uri.UnescapeDataString(route));
        }
        catch (UriFormatException)
        {
            return null;
        }
    }

    public static string? ValidateDestination(string? destination)
    {
        if (string.IsNullOrWhiteSpace(destination)) return null;

        var path = GetPath(destination);
        return AllowedDestinations.Contains(path) ? destination : null;
    }

    public static bool IsSameDestination(string first, string second) =>
        string.Equals(GetPath(first), GetPath(second), StringComparison.OrdinalIgnoreCase);

    public static bool RequiresPendingState(string destination)
    {
        var path = GetPath(destination);
        return path.EndsWith("/MarketInsight", StringComparison.OrdinalIgnoreCase)
               || path.EndsWith("/PositionListing", StringComparison.OrdinalIgnoreCase);
    }

    public static string? GetImageLocation(ImageSource? imageSource) => imageSource switch
    {
        UriImageSource uriImage => uriImage.Uri?.ToString(),
        FileImageSource fileImage => fileImage.File,
        _ => null
    };

    private static string GetPath(string destination) =>
        destination.Split('?', 2)[0].TrimEnd('/');
}
