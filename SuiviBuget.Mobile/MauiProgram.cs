using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.Services;
using SuiviBuget.Mobile.ViewModels;
using SuiviBudget.Services;
using SuiviBuget.Mobile.Interfaces;
using AlertService = SuiviBuget.Mobile.Services.AlertService;

namespace SuiviBuget.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            SQLitePCL.Batteries_V2.Init(); // <=== Initialisation SQLite
                                           // Utilisation du toolkit MAUI
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Enregistrement des services ou ViewModels
            builder.Services.AddSingleton<AppShellViewModel>();
            builder.Services.AddSingleton<PopUpMenuViewModel>();
            builder.Services.AddSingleton<LigneBudgetaireManageViewModel>();

            // Enregistrement du service de navigation
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<AlertService, AlertService>();

            // Configuration des logs en mode DEBUG
#if DEBUG
            builder.Logging.AddDebug();
#endif
            // ===== NO TINT pour tous les ImageButton =====
            Microsoft.Maui.Handlers.ImageButtonHandler.Mapper.AppendToMapping("NoTint", (handler, view) =>
            {
                #if ANDROID
                            handler.PlatformView.ImageTintList = null;
                #elif IOS || MACCATALYST
                                handler.PlatformView.TintColor = null;
                #elif WINDOWS
                            // Sur Windows MAUI, aucun tint n'est appliqué par défaut, mais si nécessaire :
                            // handler.PlatformView.Foreground = null;
                #endif
            });

            return builder.Build();
        }
    }

}
