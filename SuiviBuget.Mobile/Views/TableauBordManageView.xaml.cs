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

        try
        {
            var licence = await _service.GetLicence();
            if (licence != null)
            {
                if (licence.DateExpiration == null || licence.DateExpiration.Value.Date < DateTime.Now.Date)
                {
                    // Petite pause pour s'assurer que la page est prête
                    await Task.Delay(60);
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
                    Identifiant = Helper.GetCodeActivation(),
                    CodeActivation = "",
                    DateActivation = null,
                    DateExpiration = null,
                    IsActive = false
                };
                await _service.CreateLicenceAsync(activation);
                var vm = new SuiviBuget.Mobile.ViewModels.ActivationLicenceViewModel
                {
                    DataItem = activation
                };

                // Affiche le popup avec ce ViewModel
                this.ShowPopup(new ActivationLicenceView
                {
                    BindingContext = vm
                });
            }
        }
        catch (Exception ex )
        {

            throw ex;
        }

    }

}
