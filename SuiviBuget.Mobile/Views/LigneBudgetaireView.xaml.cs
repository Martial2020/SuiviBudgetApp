using SuiviBuget.Mobile.ViewModels;

namespace SuiviBuget.Mobile.Views;

public partial class LigneBudgetaireView : ContentPage,IQueryAttributable
{
    public LigneBudgetaireView()
	{
		InitializeComponent();
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Code", out var code))
        {
            if (BindingContext is LigneBudgetaireViewModel vm)
            {
                _ = vm.InitializePageAsync(code.ToString());
            }
        }
    }
}