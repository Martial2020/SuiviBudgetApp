using CommunityToolkit.Maui.Views;
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
            string dbPath = Helper.GetDatabaseFullPath();
            _service = new Services.Services(dbPath);
            UserAppTheme = AppTheme.Light;
            InitializeDataBase();          
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
        private async void InitializeDataBase()
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
            }
            catch (Exception ex)
            {

                throw ex;
            }
 
        }

     
    }
}