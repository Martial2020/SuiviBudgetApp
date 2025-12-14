using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.ViewModels;

namespace SuiviBuget.Mobile.Views;

public partial class RevenuManageView : ContentPage
{
	public RevenuManageView()
	{
		InitializeComponent();
	}

    private void btAction_Clicked(object sender, EventArgs e)
    {

    }

    private async void Action_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is RevenuManageModel item)
        {
            // Crée la liste des options dynamiquement
            //var options = new List<string>();
            //options.Add("Gérer cette source de revenus");   // stylo
            ////options.Add("?? Supprimer"); // poubelles

            //// Affiche l'ActionSheet
            //if (options.Count == 0)
            //{
            //    string info = await DisplayActionSheet(
            //       "Actions",
            //       "Fermer",
            //       null,
            //       "Aucune action n'est possible car il est clôturé."
            //   );
            //    return;
            //}
            //string action = await DisplayActionSheet(
            //         "Actions",
            //         "Fermer",
            //         null,
            //         options.ToArray()
            //     );
            if (BindingContext is RevenuManageViewModel vm)
            {
                //switch (action)
                //{
                //    case "Gérer cette source de revenus":
                //        vm.DetailCommand.Execute(item);
                //        break;                 
                //}
                vm.DetailCommand.Execute(item);
            }
        }
    }
}