using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Intuit.Ipp.DataService;
using MvvmHelpers;
using SQLite;
using SuiviBudget.Mobile.Interfaces;
using SuiviBudget.Services.DataAccess;
using SuiviBuget.Mobile.DataAccess;
using SuiviBuget.Mobile.Helpers;

namespace SuiviBuget.Mobile.ViewModels
{
    public class PopUpMenuViewModel : BaseViewModel
    {
        private readonly SQLiteAsyncConnection _db;
        private readonly INavigationService _navigationService;
        public ICommand LigneBudgetCommand { get; }
        public ICommand ModePaiementCommand { get; }
        public ICommand ReinitialiserCommand { get; }
        public Action? ClosePopupAction { get; set; }


        public PopUpMenuViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            LigneBudgetCommand = new RelayCommand(OnLigneBudget);
            ModePaiementCommand = new RelayCommand(OnModePaiement);
            ReinitialiserCommand = new RelayCommand(OnReinitialiser);
        }

        private async void OnReinitialiser()
        {
            ClosePopupAction?.Invoke(); // Ferme le popup
            await _navigationService.NavigateToAsync("ReinitialiserView");         
        }

        private async void OnLigneBudget()
        {
            ClosePopupAction?.Invoke(); // Ferme le popup
            await _navigationService.NavigateToAsync("LigneBudgetaireManageView");
        }

        private async void OnModePaiement()
        {
            ClosePopupAction?.Invoke(); // Ferme le popup
            await _navigationService.NavigateToAsync("ModePaiementManageView");
        }

    }
}
