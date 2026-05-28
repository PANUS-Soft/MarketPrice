namespace MarketPrice.Ui.Models
{
    public class SettingsMenuItem(
        string title,
        string? route,
        string iconSource,
        string subTitle = "",
        string badgeText = "")
    {
        public string Title { get; set; } = title;
        public string? Route { get; set; } = route;
        public string IconSource { get; set; } = iconSource;
        public string SubTitle { get; set; } = subTitle;
        public string BadgeText { get; set; } = badgeText;

        public bool HasBadge => !string.IsNullOrEmpty(BadgeText);
    }
}