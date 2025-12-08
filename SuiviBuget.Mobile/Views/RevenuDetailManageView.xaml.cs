using SuiviBuget.Mobile.ViewModels;

namespace SuiviBuget.Mobile.Views;

public partial class RevenuDetailManageView : ContentPage, IQueryAttributable
{
   
    public RevenuDetailManageView()
	{
		InitializeComponent();
	}
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Code", out var code) && query.TryGetValue("Action", out var action))
        {
            if (BindingContext is RevenuDetailManageViewModel vm)
            {
                _ = vm.InitializePageAsync(code.ToString(), action.ToString());
            }
        }
    }

    private void OnActionsRevenuDetailClicked(object sender, EventArgs e)
    {

    }
}