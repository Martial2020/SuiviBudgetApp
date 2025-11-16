using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.ViewModels;

namespace SuiviBuget.Mobile.Views;

public partial class TypeSourceRevenuManageView : ContentPage
{
    public TypeSourceRevenuManageView()
    {
        InitializeComponent();
    }

    private async void TypeRevenu_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is TypeRevenuManageModel item)
        {
            // Crée la liste des options dynamiquement
            var options = new List<string>();
            options.Add("✎ Modifier");   // stylo
            options.Add("🗑 Supprimer"); // poubelle
            if (item.EstActive)
                options.Add("⚪ Desactivé"); // poubelle
            else
                options.Add("✅ Activée"); // poubelle

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
            if (BindingContext is TypeSourceRevenuManageViewModel vm)
            {
                switch (action)
                {
                    case "✎ Modifier":
                        vm.EditCommand.Execute(item);
                        break;
                    case "🗑 Supprimer":
                        vm.DeleteCommand.Execute(item);
                        break;
                    case "⚪ Desactivé":
                    case "✅ Activée":
                        vm.StatusCommand.Execute(item);
                        break;
                }
            }
        }
    }
}