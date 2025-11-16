using SuiviBuget.Mobile.ViewModels;

namespace SuiviBuget.Mobile.Views;

public partial class TypeSourceRevenuView : ContentPage, IQueryAttributable
{
	public TypeSourceRevenuView()
	{
		InitializeComponent();
	}
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Code", out var code) && query.TryGetValue("Action", out var action))
        {
            if (BindingContext is TypeSourceRevenuViewModel vm)
            {
                _ = vm.InitializePageAsync(code.ToString(), action.ToString());
            }
        }
    }
}