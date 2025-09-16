using SuiviBuget.Mobile.Models;
using SuiviBuget.Mobile.ViewModels;

namespace SuiviBuget.Mobile.Views;

public partial class ExecutionBudgetaireManageDetailView : ContentPage, IQueryAttributable
{
	public ExecutionBudgetaireManageDetailView()
	{
		InitializeComponent();
	}
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Code", out var code) && query.TryGetValue("Action", out var action))
        {
            if (BindingContext is ExecutionBudgetaireManageDetailViewModel vm)
            {
                _ = vm.InitializePageAsync(code.ToString(), action.ToString());
            }
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
  
        if (sender is Button btn && btn.BindingContext is ExecutionBudgetaireDetailManageModel item)
        {
            // Crée la liste des options dynamiquement
            var options = new List<string>();
            options.Add("Description");
            options.Add("Supprimer");

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
            if (BindingContext is ExecutionBudgetaireManageDetailViewModel vm)
            {
                switch (action)
                {
                    case "Description":
                        vm.DescriptionCommand.Execute(item);
                        break;
                    case "Supprimer":
                        vm.DeleteCommand.Execute(item);
                        break;
                }
            }
        }
    }
}
