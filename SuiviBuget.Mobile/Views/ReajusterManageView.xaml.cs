using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.ViewModels;

namespace SuiviBuget.Mobile.Views;

public partial class ReajusterManageView : ContentPage, IQueryAttributable
{
	public ReajusterManageView()
	{
		InitializeComponent();
	}
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Code", out var code) && query.TryGetValue("Action", out var action))
        {
            if (BindingContext is ReajusterManageViewModel vm)
            {
                vm.InitializePageAsync(code.ToString(), action.ToString());
            }
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is ReajusterManageModel item)
        {
            // Crée la liste des options dynamiquement
            var options = new List<string>();
            //options.Add("Description");
            options.Add("🗑 Supprimer");          // poubelle

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
            if (BindingContext is ReajusterManageViewModel vm)
            {
                switch (action)
                {
                    case "🗑 Supprimer":
                        vm.DeleteCommand.Execute(item);
                        break;
                }
            }
        }

    }
}