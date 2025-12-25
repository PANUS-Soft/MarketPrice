using Microsoft.Extensions.Logging;
using DevExpress.Maui;

namespace MarketPrice.Ui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
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

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
