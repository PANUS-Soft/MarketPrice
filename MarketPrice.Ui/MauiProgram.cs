using DevExpress.Maui;
using CommunityToolkit.Maui;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;
using MarketPrice.Ui.ViewModels;
using MarketPrice.Ui.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using CommunityToolkit.Maui.Core;

namespace MarketPrice.Ui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitCore()
                .UseDevExpress(useLocalization: false)
                .UseDevExpressCharts()
                .UseDevExpressCollectionView()
                .UseDevExpressControls()
                .UseDevExpressDataGrid()
                .UseDevExpressEditors()
                .UseDevExpressTreeView()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("RobotoSerif-Bold.ttf", "RobotoSerifBold");
                    fonts.AddFont("RobotoSerif-Light.ttf", "RobotoSerifLight");
                    fonts.AddFont("RobotoSerif-Medium.ttf", "RobotoSerifMedium");
                    fonts.AddFont("RobotoSerif-Regular.ttf", "RobotoSerifRegular");
                    fonts.AddFont("RobotoSerif-SemiBold.ttf", "RobotoSerifSemibold");
                });

            builder.AddAppSettings();

            string marketPriceApiBaseUrl = builder.Configuration.GetValue<string>("MarketPriceApiBaseUrl")
                ?? throw new InvalidOperationException("MarketPrice API Base URL is missing ... Couldn't be loaded.");

            builder.Services.AddSingleton(sp =>
            {
                return new HttpClient
                {
                    BaseAddress = new Uri(marketPriceApiBaseUrl)
                };
            });

            // Register application services
            builder.Services.AddSingleton<AuthenticationApiService>();
            builder.Services.AddSingleton<SessionService>();
            builder.Services.AddSingleton<SessionStorage>();

            // Register view models
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<LoginViewModel>();

            // Register views
            builder.Services.AddTransient<Register>();
            builder.Services.AddTransient<Login>();



#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static void AddAppSettings(this MauiAppBuilder builder)
        {
            using Stream stream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream("MarketPrice.Ui.appsettings.json");

            if (stream != null)
            {
                IConfigurationRoot config = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();
                builder.Configuration.AddConfiguration(config);
            }

        }
    }

}
