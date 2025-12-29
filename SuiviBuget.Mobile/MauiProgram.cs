using CommunityToolkit.Maui;
using Microcharts.Maui;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using SuiviBudget.Mobile.Interfaces;
using SuiviBudget.Services;
using SuiviBuget.Mobile.Interfaces;
using SuiviBuget.Mobile.Services;
using SuiviBuget.Mobile.ViewModels;
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
                 .UseLocalNotification() // 🔔 Ajout ici
                .UseMauiCommunityToolkit()
                .UseMicrocharts()  // <-- c'est cette ligne qui manquait
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            // autres configurations...
            // Enregistrement des services ou ViewModels
            builder.Services.AddSingleton<AppShellViewModel>();
            builder.Services.AddSingleton<PopUpMenuViewModel>();
            //builder.Services.AddSingleton<ExecutionBudgetaireDetailManageViewModel>();

            // Enregistrement du service de navigation
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<IAlertService, AlertService>();
            builder.Services.AddSingleton<IDialogService, DialogService>();
            //builder.UseMauiApp<App>().UseLocalNotification();
            // Configuration des logs en mode DEBUG
#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }

}
