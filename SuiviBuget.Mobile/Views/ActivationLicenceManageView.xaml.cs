using SuiviBuget.Mobile.DataAccess;
using SuiviBuget.Mobile.ViewModels;

namespace SuiviBuget.Mobile.Views;

public partial class ActivationLicenceManageView : ContentPage
{
	public ActivationLicenceManageView()
	{
		InitializeComponent();
	}
    private async void OnActionsLicenceClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is ActivationLicence item)
        {
            // Crée la liste des options dynamiquement
            var options = new List<string> { "📋 Copier le code d'activation", "🗑 Supprimer" };
            // Affiche l'ActionSheet
            if (options.Count == 0)
            {
                string info = await DisplayActionSheet(
                   "Actions",
                   "Fermer",
                   null
               );
                return;
            }
            string action = await DisplayActionSheet(
                     "Actions",
                     "Fermer",
                     null,
                     options.ToArray()
                 );
            if (BindingContext is ActivationLicenceManageViewModel vm)
            {
                switch (action)
                {
                    case "🗑 Supprimer":
                        vm.DeleteLicenceCommand.Execute(item);
                        break;
                    case "📋 Copier le code d'activation":
                        vm.CopyCodeActivationCommand.Execute(item);
                        break;
                }
            }
        }
    }

}