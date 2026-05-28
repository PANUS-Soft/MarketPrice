using MarketPrice.Ui.Services.Session;
using MarketPrice.Ui.ViewModels;
using MarketPrice.Ui.Views;

namespace MarketPrice.Ui
{
    public partial class App : Application
    {
        private readonly SessionService _sessionService;

        public App(SessionService sessionService, ActivityViewModel activityViewModel)
        {
            InitializeComponent();
            _sessionService = sessionService;

            // 1. Immediately show the SplashPage to match the OS loading screen
            MainPage = new SplashScreen();
            //MainPage = new NavigationPage(new Views.Activity(activityViewModel));
        }

        protected override async void OnStart()
        {
            base.OnStart();

            try
            {
                // 2. Run all background checks while SplashPage is active
                await _sessionService.InitializeAsync();

                // We check these two variables to decide the path
                var hasCompletedOnboarding = Preferences.Get("HasCompletedOnboarding", false);
                bool hasValidSession = await _sessionService.ValidateAndRefreshSessionAsync();

                // 3. Initialize the Shell (but don't show it yet)
                MainPage = new AppShell();

                // 4. Perform the "Silent Navigation"
                if (!hasCompletedOnboarding)
                {
                    // Case A: Brand New User -> Onboarding
                    await Shell.Current.GoToAsync("//Onboarding");
                }
                else if (hasValidSession)
                {
                    // Case B: Returning User with active session -> Home
                    await Shell.Current.GoToAsync("//Home");
                }
                else
                {
                    // Case C: Returning User with expired session -> Welcome
                    await Shell.Current.GoToAsync("//Welcome");
                }
            }
            catch (Exception)
            {
                // Safety net: If anything fails, go to Welcome
                MainPage = new AppShell();
                await Shell.Current.GoToAsync("//Welcome");
            }
        }
    }
}