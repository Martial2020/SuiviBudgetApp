using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using SuiviBudget.Mobile.Constants;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.DataAccess;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.ViewModels;
using SuiviBuget.Mobile.Views.Popups;

namespace SuiviBuget.Mobile.Views;

public partial class BudgetManageView : ContentPage
{
    IServices _service { get; set; }
    public BudgetManageView()
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
        catch (Exception ex)
        {

            throw ex;
        }


    }

    private async void OnActionsClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is BudgetManageModel item)
        {
            // Crée la liste des options dynamiquement
            var options = new List<string>();
            if (item.StatutBudget == StatutBudgetConst.Ouvert)
            {
                //options.Add("🔄 Réinitialiser");     // flèche circulaire pour réinitialiser
                //options.Add("📂 Type de dépense");    // dossier pour type
                //options.Add("💳 Mode de paiement");   // carte pour paiement
                options.Add("✎ Modifier");   // stylo
                options.Add("🗑 Supprimer"); // poubelle
                options.Add("📄 Détails du budget");
                options.Add("💰 Allocation budgétaire");
                options.Add("⏳ Mettre en cours");
            }
            else if (item.StatutBudget == StatutBudgetConst.Encours)
            {
                options.Add("✎ Modifier");   // stylo
                options.Add("🗑 Supprimer"); // poubelle
                options.Add("📄 Détails du budget");
                options.Add("💰 Allocation budgétaire");
                options.Add("♻️ Réajustement budgétaire");              
                options.Add("🔒 Clôturer");
            }
            else
            {
                
                options.Add("📄 Détails du budget");
                options.Add("💰 Allocation budgétaire");
                options.Add("♻️ Réajustement budgétaire");
            }
            //else
            //{
            //    options.Add("Supprimer");
            //}

            // Affiche l'ActionSheet
            if (options.Count == 0)
            {
                string info = await DisplayActionSheet(
                   "Actions sur le budget",
                   "Fermer",
                   null,
                   "Aucune action n'est possible car il est clôturé."
               );
                return;
            }
            string action = await DisplayActionSheet(
                     "Actions sur le budget",
                     "Fermer",
                     null,
                     options.ToArray()
                 );
            if (BindingContext is BudgetManageViewModel vm)
            {
                switch (action)
                {
                    case "✎ Modifier":
                        vm.EditCommand.Execute(item);
                        break;
                    case "🗑 Supprimer":
                        vm.DeleteCommand.Execute(item);
                        break;
                    case "🔒 Clôturer":
                        vm.CloturerCommand.Execute(item);
                        break;
                    case "⏳ Mettre en cours":
                        vm.EncoursCommand.Execute(item);
                        break;
                    case "💰 Allocation budgétaire":
                        vm.BudgetDetailCommand.Execute(item);
                        break;
                    case "♻️ Réajustement budgétaire":
                        vm.ReajusterCommand.Execute(item);
                        break;
                    case "📄 Détails du budget":
                        vm.DetailsBudgetCommand.Execute(item);
                        break;

                }
            }
        }
    }

}