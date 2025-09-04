using CommunityToolkit.Maui.Views;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using SuiviBuget.Mobile.Services;
using System.Diagnostics;



namespace SuiviBuget.Mobile.Views.Popups;

public partial class PopUpMenuView : Popup
{
    INavigationService navigationService;
    public PopUpMenuView()
    {
        InitializeComponent();
        try
        {
            navigationService = new NavigationService();
            // Créez une instance du ViewModel
            var viewModel = new PopUpMenuViewModel(navigationService)
            {
                ClosePopupAction = () =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        this.Close(); // ferme le popup proprement
                    });
                }
            };

            // Associez le ViewModel au BindingContext du Popup
            BindingContext = viewModel;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERREUR : " + ex.InnerException?.Message ?? ex.Message);
        }
        

    }
}
