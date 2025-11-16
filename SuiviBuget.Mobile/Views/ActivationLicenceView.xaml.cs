using CommunityToolkit.Maui.Views;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.ViewModels;
using System.Diagnostics;

namespace SuiviBuget.Mobile.Views.Popups;

public partial class ActivationLicenceView : Popup
{
    public ActivationLicenceView()
    {
        InitializeComponent();

        try
        {
            // Lorsque le BindingContext est défini (depuis le tableau de bord),
            // on associe la méthode pour fermer le popup proprement.
            BindingContextChanged += (s, e) =>
            {
                if (BindingContext is ActivationLicenceViewModel vm)
                {
                    vm.ClosePopupAction = () =>
                    {
                        MainThread.BeginInvokeOnMainThread(() => Close());
                    };
                }
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERREUR : " + (ex.InnerException?.Message ?? ex.Message));
        }
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        Helper.OpenWhatsApp();
    }
}
