using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketPrice.Ui.Models;
using MarketPrice.Ui.Services.Session;
using System.Collections.ObjectModel;

namespace MarketPrice.Ui.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly SessionService _sessionService;

        [ObservableProperty] private string fullName;
        [ObservableProperty] private string phoneNumber;

        public ObservableCollection<SettingsMenuItem> SettingsItems { get; } = new();

        public SettingsViewModel(SessionService sessionService)
        {
            _sessionService = sessionService;

            LoadHeader();
            LoadSettings();
        }

        private async void LoadHeader()
        {
            var session = await _sessionService.GetCurrentSessionAsync();

            if (session == null)
                return;

            FullName = $"{session.FirstName} {session.FamilyName}";
            PhoneNumber = session.PhoneNumber;
        }

        private void LoadSettings()
        {
            SettingsItems.Clear();

            SettingsItems.Add(new SettingsMenuItem(
                "Notifications",
                "Notifications",
                "notification_icon.png",
                "Manage alerts and updates"));

            SettingsItems.Add(new SettingsMenuItem(
                "Privacy",
                "Privacy",
                "privacy_icon.png",
                "Control account privacy"));

            SettingsItems.Add(new SettingsMenuItem(
                "Appearance",
                "Appearance",
                "theme_icon.png",
                "Dark mode and themes"));

            SettingsItems.Add(new SettingsMenuItem(
                "Language",
                "Language",
                "language_icon.png",
                "Choose your preferred language"));

            SettingsItems.Add(new SettingsMenuItem(
                "Help & Support",
                "Support",
                "support_icon.png",
                "Need assistance?"));
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task OpenMenuAsync()
        {
            await Shell.Current.DisplayActionSheet(
                "Options",
                "Cancel",
                null,
                "Refresh",
                "About");
        }

        [RelayCommand]
        private async Task NavigateToItemAsync(SettingsMenuItem? item)
        {
            if (item == null || string.IsNullOrEmpty(item.Route))
                return;

            await Shell.Current.GoToAsync(item.Route);
        }
    }
}