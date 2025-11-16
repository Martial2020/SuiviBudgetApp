using SuiviBuget.Mobile.Helpers;

namespace SuiviBuget.Mobile.Views;

public partial class AproposView : ContentPage
{
	public AproposView()
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
        Helper.OpenWhatsApp();

    }
}