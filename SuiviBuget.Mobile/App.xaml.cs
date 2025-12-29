using CommunityToolkit.Maui.Views;
using Plugin.LocalNotification;
using SQLite;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.DataAccess;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Services;
using SuiviBuget.Mobile.Views.Popups;

namespace SuiviBuget.Mobile
{
    public partial class App : Application
    {
        IServices _service { get; set; }
        public App()
        {
            InitializeComponent();

            // 1️⃣ Définir le chemin de la base de données et initialiser le service
            string dbPath = Helper.GetDatabaseFullPath();
            _service = new Services.Services(dbPath);

            UserAppTheme = AppTheme.Light;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());

            // 🔥 POINT IDÉAL POUR INITIALISER
            _ = InitializeAppAsync();

            return window;
        }

        private async Task InitializeAppAsync()
        {
            try
            {
                // 1️⃣ Initialisation DB
                await _service.InitDatabaseAsync();

                // 2️⃣ Licence
                var existing = await _service.GetLicence();
                if (existing == null)
                {
                    var activation = new Licence
                    {
                        Identifiant = Helper.GetCodeActivation(),
                        IsActive = false
                    };

                    await _service.CreateLicenceAsync(activation);
                }

                // 3️⃣ Devises
                await _service.AddOrUpdateDevise();

                // 4️⃣ 🔔 PLANIFICATION DES NOTIFICATIONS (UNE FOIS)
                await Helper.PlanifierNotificationsQuotidiennesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }

}