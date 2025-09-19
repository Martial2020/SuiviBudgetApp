using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.ViewModels;

namespace SuiviBuget.Mobile.Views;

public partial class LigneBudgetaireManageView : ContentPage
{
	public LigneBudgetaireManageView()
	{
		InitializeComponent();	
       
    }
    private async void OnActionsLigneClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is LigneBudgetaireManageModel item)
        {
            // Crée la liste des options dynamiquement
            var options = new List<string>();
            options.Add("✎ Modifier");   // stylo
            options.Add("🗑 Supprimer"); // poubelle

            // Affiche l'ActionSheet
            if (options.Count == 0)
            {
                string info = await DisplayActionSheet(
                   "Actions",
                   "Fermer",
                   null,
                   "Aucune action n'est possible car il est clôturé."
               );
                return;
            }
            string action = await DisplayActionSheet(
                     "Actions",
                     "Fermer",
                     null,
                     options.ToArray()
                 );
            if (BindingContext is LigneBudgetaireManageViewModel vm)
            {
                switch (action)
                {
                    case "✎ Modifier":
                        vm.EditCommand.Execute(item);
                        break;
                    case "🗑 Supprimer":
                        vm.DeleteCommand.Execute(item);
                        break;
                }
            }
        }
    }
}