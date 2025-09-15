using SuiviBuget.Mobile.ViewModels;

namespace SuiviBuget.Mobile.Views;

public partial class ExecutionBudgetaireManageView : ContentPage
{
    public ExecutionBudgetaireManageViewModel vm { get; }
    public ExecutionBudgetaireManageView()
	{
		InitializeComponent();
        // Instanciation du ViewModel
       vm = new ExecutionBudgetaireManageViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Recharger la liste
        _=vm.LoadBudgetAsync("");
    }


}