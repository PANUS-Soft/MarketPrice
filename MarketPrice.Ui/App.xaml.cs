using MarketPrice.Ui.Services.Session;

namespace MarketPrice.Ui
{
    public partial class App : Application
    {
        private readonly SessionService _sessionService;

        public App(SessionService sessionService)
        {
            InitializeComponent();

            _sessionService = sessionService;
        
            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            base.OnStart();
            await _sessionService.InitializeAsync();
            await _sessionService.ValidateAndRefreshSessionAsync();

            try
            {
                await HandleStartupNavigationAsync();
            }
            catch
            {
                await Shell.Current.GoToAsync("//Welcome");
            }
        }

        private async Task HandleStartupNavigationAsync()
        {
            await Shell.Current.GoToAsync("//EditProfile");
            return;
            //var hasOnboarded = Preferences.Get("HasCompletedOnboarding", false);
            //if (!hasOnboarded)
            //{
            //    await Shell.Current.GoToAsync("//Onboarding");
            //    return;
            //}

            //bool hasValidSession = await _sessionService.ValidateAndRefreshSessionAsync();

            //if (hasValidSession)
            //{
            //    await Shell.Current.GoToAsync("//Home");
            //}
            //else
            //{
            //    await Shell.Current.GoToAsync("//Welcome");
            //}
        }

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    return new Window(new AppShell());
        //}
    }
}