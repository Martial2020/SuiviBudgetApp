using SuiviBuget.Mobile.ViewModels;

namespace SuiviBuget.Mobile.Views;

public partial class ExecutionBudgetaireDetailView : ContentPage, IQueryAttributable
{
	public ExecutionBudgetaireDetailView()
	{
		InitializeComponent();
	}
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Code", out var code) && query.TryGetValue("Action", out var action))
        {
            if (BindingContext is ExecutionBudgetaireDetailViewModel vm)
            {
                _ = vm.InitializePageAsync(code.ToString(), action.ToString());
            }
        }
    }
}