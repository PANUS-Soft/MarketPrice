using MarketPrice.Ui.Services.Session;
using MarketPrice.Ui.Views;

namespace MarketPrice.Ui
{
    public partial class App : Application
    {
        private readonly SessionService _sessionService;

        public App(SessionService sessionService)
        {
            InitializeComponent();
            _sessionService = sessionService;

            // 1. Immediately show the SplashPage to match the OS loading screen
            MainPage = new SplashScreen();
        }

        protected override async void OnStart()
        {
            base.OnStart();

            try
            {
                // 2. Run all background checks while SplashPage is active
                await _sessionService.InitializeAsync();

                //// We check these two variables to decide the path
                //var hasCompletedOnboarding = Preferences.Get("HasCompletedOnboarding", false);

                // 3. Initialize the Shell (but don't show it yet)
                MainPage = new AppShell();

                // Always go to Market first
                await Shell.Current.GoToAsync("//Market");
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