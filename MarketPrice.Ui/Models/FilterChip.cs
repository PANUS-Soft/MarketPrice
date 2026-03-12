using CommunityToolkit.Mvvm.ComponentModel;

namespace MarketPrice.Ui.Models;

public partial class FilterChip : ObservableObject
{
    public string Title { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BackgroundColor))]
    [NotifyPropertyChangedFor(nameof(TextColor))]
    [NotifyPropertyChangedFor(nameof(BorderColor))]
    private bool isSelected;
    public Color BackgroundColor => IsSelected ? Color.FromArgb("#007BFF") : Colors.White;
    public Color TextColor => IsSelected ? Colors.White : Color.FromArgb("#007BFF");
    public Color BorderColor => IsSelected ? Colors.Transparent : Color.FromArgb("#007BFF");
}