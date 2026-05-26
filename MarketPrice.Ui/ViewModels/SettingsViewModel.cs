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

            FullName = "Tertullien Chesseu";
            PhoneNumber = "+237 699 53 89 39";
        }

        private void LoadSettings()
        {
            SettingsItems.Clear();

            SettingsItems.Add(new SettingsMenuItem(
                "Notifications",
                "Notifications",
                "notification_icon",
                "Manage alerts and updates"));

            SettingsItems.Add(new SettingsMenuItem(
                "Privacy",
                "Privacy",
                "privacy_and_security_icon",
                "Control account privacy"));

            SettingsItems.Add(new SettingsMenuItem(
                "Appearance",
                "Appearance",
                "theme_icon",
                "Dark mode and themes"));

            SettingsItems.Add(new SettingsMenuItem(
                "Language",
                "Language",
                "language_icon",
                "Choose your preferred language"));

            SettingsItems.Add(new SettingsMenuItem(
                "Help & Support",
                "Support",
                "support_icon",
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