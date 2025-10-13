using CommunityToolkit.Maui.Views;
using Microcharts.Maui;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.DataAccess;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.ViewModels;
using SuiviBuget.Mobile.Views.Popups;

namespace SuiviBuget.Mobile.Views;

public partial class TableauBordManageView : ContentPage
{
    IServices _service { get; set; }

    public TableauBordManageView()
	{
		InitializeComponent();
        string dbPath = Helper.GetDatabaseFullPath();
        _service = new Services.Services(dbPath);
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var licence = await _service.GetLicence();
        if (licence != null)
        {          
            if (licence.DateExpiration == null || licence.DateExpiration < DateTime.Now.Date)
            {
                // Petite pause pour s'assurer que la page est prête
                await Task.Delay(200);
                licence.CodeActivation = "";
                // Crée le ViewModel et initialise ses propriétés
                var vm = new SuiviBuget.Mobile.ViewModels.ActivationLicenceViewModel
                {
                    DataItem = licence
                };

                // Affiche le popup avec ce ViewModel
                this.ShowPopup(new ActivationLicenceView
                {
                    BindingContext = vm
                });
            }
        }
        else
        {
            var activation = new Licence
            {
                Identifiant= Helper.GetCodeActivation(),
                CodeActivation = null,
                DateActivation = null,
                DateExpiration = null,
                IsActive = false
            };
            await _service.CreateLicenceAsync(activation);
        }





        
    }

}
