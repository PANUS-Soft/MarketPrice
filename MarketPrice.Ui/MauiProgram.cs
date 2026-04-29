using DevExpress.Maui;
using Syncfusion.Maui.Toolkit.Hosting;
using CommunityToolkit.Maui;
using MarketPrice.Ui.Services.Api;
using MarketPrice.Ui.Services.Session;
using MarketPrice.Ui.ViewModels;
using MarketPrice.Ui.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using CommunityToolkit.Maui.Core;
using MarketPrice.Ui.Common;

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
                .ConfigureSyncfusionToolkit()
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

            builder.Services.AddOptions<ApiSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("ApiSettings").Bind(settings);
                });


            // Register application services
            builder.Services.AddSingleton<AuthenticationApiService>();
            builder.Services.AddSingleton<SessionService>();
            builder.Services.AddTransient<AuthHandler>();

            // 2. Apply it to ALL HttpClients
            builder.Services.ConfigureHttpClientDefaults(builder =>
            {
                builder.AddHttpMessageHandler<AuthHandler>();
            });

            builder.Services.AddHttpClient<AuthenticationApiService>();
            builder.Services.AddHttpClient<ReferenceDataApiService>();
            builder.Services.AddHttpClient<PositionApiService>();
            builder.Services.AddHttpClient<HomeApiService>();
            builder.Services.AddHttpClient<MarketApiService>();
            builder.Services.AddHttpClient<ProfileApiService>();
            builder.Services.AddHttpClient<ActivityApiService>();

            // Register view models
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<MarketViewModel>();
            builder.Services.AddTransient<MarketInsightViewModel>();
            builder.Services.AddTransient<PlacePositionViewModel>();
            builder.Services.AddTransient<PositionListingViewModel>();
            builder.Services.AddTransient<PositionDetailViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<EditProfileViewModel>();
            builder.Services.AddTransient<ChangePasswordViewModel>();
            builder.Services.AddTransient<ActivityViewModel>();

            // Register views
            builder.Services.AddTransient<Register>();
            builder.Services.AddTransient<Login>();
            builder.Services.AddTransient<Home>();
            builder.Services.AddTransient<Market>();
            builder.Services.AddTransient<MarketInsight>();
            builder.Services.AddTransient<PlacePosition>();
            builder.Services.AddTransient<PositionListing>();
            builder.Services.AddTransient<PositionDetail>();
            builder.Services.AddTransient<Profile>();
            builder.Services.AddTransient<EditProfile>();
            builder.Services.AddTransient<ChangePassword>();
            builder.Services.AddTransient<Activity>();

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
