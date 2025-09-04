using Microsoft.Maui.Controls;
using SuiviBudget.Mobile.Constants;
using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.ViewModels;

namespace SuiviBuget.Mobile.Views;

public partial class BudgetManageView : ContentPage
{
    public BudgetManageView()
    {
        InitializeComponent();
    }
    private async void OnActionsClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is BudgetManageModel item)
        {
            // Cr�e la liste des options dynamiquement
            var options = new List<string>();
            if (item.StatutBudget == StatutBudgetConst.Ouvert)
            {

                options.Add("Modifier");
                options.Add("Supprimer");              
                options.Add("En cours");               
                // Ne pas ajouter "Cl�turer"
            }
            else if (item.StatutBudget == StatutBudgetConst.Encours)
            {
                options.Add("Modifier");
                options.Add("Supprimer");
                options.Add("Cl�turer");
                
            }
            else
            {
                options.Add("Supprimer");
            }
            options.Add("D�tails");

            // Affiche l'ActionSheet
            if (options.Count == 0)
            {
                string info = await DisplayActionSheet(
                   "Actions sur le budget",
                   "Fermer",
                   null,
                   "Aucune action n'est possible car il est cl�tur�."
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
                    case "Modifier":
                        vm.EditCommand.Execute(item);
                        break;
                    case "Supprimer":
                        vm.DeleteCommand.Execute(item);
                        break;
                    case "Cl�turer":
                        vm.CloturerCommand.Execute(item);
                        break;
                    case "En cours":
                        vm.EncoursCommand.Execute(item);
                        break;
                    case "D�tails":
                        vm.BudgetDetailCommand.Execute(item);
                        break;
                }
            }
        }
    }

}