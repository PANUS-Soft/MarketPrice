using MarketPrice.Ui.Services.Session;

namespace MarketPrice.Ui
{
    public partial class App : Application
    {
        private readonly SessionService _sessionService;
        private readonly SessionStorage _sessionStorage;

        public App(SessionService sessionService, SessionStorage sessionStorage)
        {
            InitializeComponent();

            _sessionService = sessionService;
            _sessionStorage = sessionStorage;

            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            base.OnStart();

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
            var hasOnboarded = Preferences.Get("HasCompletedOnboarding", false);
            if (!hasOnboarded)
            {
                await Shell.Current.GoToAsync("//Onboarding");
                return;
            }

            var session = await _sessionStorage.LoadAsync();
            if (session != null && session.ExpireAt > DateTime.Now)
            {
                _sessionService.StartSession(session);
                await Shell.Current.GoToAsync("//Home");
            }
            else
            {
                await Shell.Current.GoToAsync("//Welcome");
            }
        }

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    return new Window(new AppShell());
        //}
    }
}