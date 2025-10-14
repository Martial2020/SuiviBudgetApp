using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.ViewModels;

namespace SuiviBuget.Mobile.Views;

public partial class ModePaiementManageView : ContentPage
{
	public ModePaiementManageView()
	{
		InitializeComponent();	     
    }
    private async void OnActionsModePaiementClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is ModePaiementManageModel item)
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
            if (BindingContext is ModePaiementManageViewModel vm)
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