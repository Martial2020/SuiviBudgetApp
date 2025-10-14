using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SuiviBudget.Mobile.Interfaces;
using SuiviBuget.Mobile.Helpers;
using SuiviBuget.Mobile.Services;
using static SuiviBuget.Mobile.Messages.Messages;

namespace SuiviBuget.Mobile.ViewModels
{
    public class ReinitialiserViewModel : ObservableObject
    {
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }

        IServices service { get; set; }
        public ICommand ReinitialiserCommand { get; }
        private readonly INavigationService _navigationService;

        public ReinitialiserViewModel()
        {
            var dbPath = Helper.GetDatabaseFullPath();
            service = new Services.Services(dbPath);
            _navigationService = new NavigationService();
            ReinitialiserCommand = new RelayCommand(OnReinitialiserCommand);
        }

        private async void OnReinitialiserCommand()
        {
            var confirm = await Shell.Current.CurrentPage.DisplayAlert(
           "Réinitialisation", "Êtes-vous sur(e) de vouloir réinitialiser votre application ?",
            "Oui", "Non");

            if (!confirm) return;
            IsBusy = true;
            await Task.Delay(5); // Simule un temps de chargement
            service.ReinitialiseApp();
            //// Prévenir tous les ViewModels
            WeakReferenceMessenger.Default.Send(new ResetAppMessage());
            Application.Current.MainPage = new AppShell();
        }
    }
}
