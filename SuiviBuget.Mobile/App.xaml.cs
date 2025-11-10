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

            // 2️⃣ Définir le thème
            UserAppTheme = AppTheme.Light;

            // 3️⃣ Créer la MainPage
            //MainPage = new AppShell(); // ou ta page principale

            // 4️⃣ Lancer l'initialisation de la DB en tâche de fond
            _ = InitializeDataBase();

            // 5️⃣ Lancer la planification des notifications en tâche de fond
           _ = Helper.PlanifierNotificationsQuotidiennes();
        }



        protected override Window CreateWindow(IActivationState? activationState)
        {
            //return new Window(new AppShell());
            var window = new Window(new AppShell());

            window.Dispatcher.Dispatch(async () =>
            {
              
                await Helper.PlanifierNotificationsQuotidiennes(); // Replanifie proprement
            });

            return window;
        }
        private async Task InitializeDataBase()
        {
            try
            {
                //Verifier si le code d'activation est dejà crée
                var existing = await _service.GetLicence();
                if (existing == null)
                {
                    var activation = new Licence
                    {
                        Identifiant = Helper.GetCodeActivation(),
                        CodeActivation = "",
                        DateActivation = null,
                        DateExpiration = null,
                        IsActive = false
                    };
                    await _service.CreateLicenceAsync(activation);
                }


                //Charger les devises
                await _service.AddOrUpdateDevise();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


    }
}