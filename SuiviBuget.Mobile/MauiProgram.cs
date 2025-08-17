using Budget.Services.Services;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SuiviBudget.Core.Interfaces;
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

            SQLitePCL.Batteries_V2.Init(); // <=== Ajoute ceci
            // Utilisation du toolkit MAUI (doit être appelé avant la configuration des autres services)
            builder
                .UseMauiApp<App>()  // Assurez-vous que vous utilisez le bon point d'entrée pour l'application MAUI
                .UseMauiCommunityToolkit() // L'intégration du Toolkit MAUI
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Enregistrement des services ou ViewModels
            builder.Services.AddSingleton<AppShellViewModel>();  // Si tu utilises un ViewModel pour AppShell
            builder.Services.AddSingleton<PopUpMenuViewModel>(); // Enregistrer PopUpMenuViewModel
            builder.Services.AddSingleton<LigneBudgetaireManageViewModel>(); // Enregistrer PopUpMenuViewModel

            // Enregistrement du service de navigation
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<AlertService, AlertService>();

            // Configuration des logs en mode DEBUG
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
