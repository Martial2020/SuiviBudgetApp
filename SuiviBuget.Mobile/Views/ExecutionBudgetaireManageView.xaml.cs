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

   

}