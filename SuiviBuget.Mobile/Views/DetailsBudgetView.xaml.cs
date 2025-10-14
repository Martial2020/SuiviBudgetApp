using SuiviBuget.Mobile.ViewModels;

namespace SuiviBuget.Mobile.Views;

public partial class DetailsBudgetView : ContentPage, IQueryAttributable
{
	public DetailsBudgetView()
	{
		InitializeComponent();
	}
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Code", out var code) && query.TryGetValue("Action", out var action))
        {
            if (BindingContext is DetailsBudgetViewModel vm)
            {
                _ = vm.InitializePageAsync(code.ToString(), action.ToString());
            }
        }
    }


}