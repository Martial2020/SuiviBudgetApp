using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuiviBudget.Mobile.Interfaces;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using MvvmHelpers;

namespace SuiviBuget.Mobile.ViewModels
{
    public class PopUpMenuViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        public ICommand LigneBudgetCommand { get; }
        public ICommand ParametreCommand { get; }
        public Action? ClosePopupAction { get; set; }

        public PopUpMenuViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            LigneBudgetCommand = new RelayCommand(OnLigneBudget);
            ParametreCommand = new RelayCommand(OnParametreCommand);
        }
        private async void OnLigneBudget()
        {
            ClosePopupAction?.Invoke(); // Ferme le popup
            // Navigation vers la page LigneBudgetPage
            await _navigationService.NavigateToAsync("LigneBudgetaireManageView");
        }

        private async void OnParametreCommand()
        {
            ClosePopupAction?.Invoke(); // Ferme le popup
            // Navigation vers la page LigneBudgetPage
            await _navigationService.NavigateToAsync("ParametreManageView");
        }

    }
}
