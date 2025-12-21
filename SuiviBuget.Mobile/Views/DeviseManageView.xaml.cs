using SuiviBuget.Mobile.DataAccess;
using SuiviBuget.Mobile.ViewModels;

namespace SuiviBuget.Mobile.Views;

public partial class DeviseManageView : ContentPage
{
    public DeviseManageView()
    {
        InitializeComponent();
    }

    private async void Devise_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is Devise item)
        {
            //// Crée la liste des options dynamiquement
            //var options = new List<string>();

            //if (!item.EstActive)
            //{
            //    options.Add("💱 Changer la devise");   // stylo
            //}
            //if (options.Count == 0)
            //{
            //    string info = await DisplayActionSheet(
            //       "Actions",
            //       "Fermer",
            //       null,
            //       "Cette devise est dejà en cours d'utilisation."
            //   );
            //    return;
            //}
            //string action = await DisplayActionSheet(
            //         "Actions",
            //         "Fermer",
            //         null,
            //         options.ToArray()
            //     );
            if (BindingContext is DeviseManageViewModel vm)
                vm.UtiliseCommand.Execute(item);
        }
    }
}