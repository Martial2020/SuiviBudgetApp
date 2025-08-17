using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using System.Windows.Input;
using SuiviBuget.Mobile.Views.Popups;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SuiviBuget.Mobile.ViewModels
{
    public class AppShellViewModel : ObservableObject
    {
        // Commande pour afficher le menu
        public ICommand ShowMenuCommand { get; }

        public AppShellViewModel()
        {
            // Initialiser la commande
            ShowMenuCommand = new RelayCommand(OnShowMenuClicked);
        }

        private async void OnShowMenuClicked()
        {
            // Crée et affiche le PopUp ici
            var menuPopup = new PopUpMenuView();
            await Shell.Current.CurrentPage.ShowPopupAsync(menuPopup);
        }
    }
}
